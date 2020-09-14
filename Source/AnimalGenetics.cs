using System.Collections.Generic;
using System;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;

namespace AnimalGenetics
{
    public class AnimalGenetics : WorldComponent
    {
        public static StatDef GatherYield = new StatDef { defName = "GatherYield", description = "AG.GatherYieldDesc".Translate(), alwaysHide = true };
        public static StatDef Damage = new StatDef { defName = "Damage", description = "AG.DamageDesc".Translate(),alwaysHide = true };
        public static StatDef Health = new StatDef { defName = "Health", description = "AG.HealthDesc".Translate(), alwaysHide = true };

        public AnimalGenetics(World world) : base(world)
        {

        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref _Data, "data", LookMode.Reference, LookMode.Deep, ref _Things, ref _StatGroups);
        }

        Dictionary<Thing, StatGroup> _Data = new Dictionary<Thing, StatGroup>();
        List<Thing> _Things = new List<Thing>();
        List<StatGroup> _StatGroups = new List<StatGroup>();

        private StatGroup GetData(Pawn pawn)
        {
            if (!_Data.ContainsKey(pawn))
                _Data[pawn] = GenerateStatsGroup(pawn);
            return _Data[pawn];
        }

        public StatRecord GetFactor(Pawn pawn, StatDef stat)
        {
            return GetData(pawn).GetFactor(stat);
        }

        public StatGroup GenerateStatsGroup(Pawn pawn)
        {
            StatGroup toReturn = new StatGroup();

            if (!Genes.EffectsThing(pawn))
                return toReturn;

            var mother = pawn.GetMother();
            var father = pawn.GetFather();

            var motherStats = mother == null ? null : GetData(mother);
            var fatherStats = father == null ? null : GetData(father);

            var affectedStats = Constants.affectedStats;

            foreach (var stat in affectedStats)
            {
                var record = new StatRecord();

                float motherValue = motherStats != null ? motherStats.GetFactor(stat).Value : Utilities.SampleGaussian(Controller.Settings.mean, Controller.Settings.stdDev, 0.1f);
                float fatherValue = fatherStats != null ? fatherStats.GetFactor(stat).Value : Utilities.SampleGaussian(Controller.Settings.mean, Controller.Settings.stdDev, 0.1f);

                bool fromMother = Utilities.SampleInt() % 2 == 0;

                if (fromMother)
                {
                    record.ParentValue = motherValue;
                    record.Parent = motherStats != null ? StatRecord.Source.Mother : StatRecord.Source.None;
                }
                else
                {
                    record.ParentValue = fatherValue;
                    record.Parent = fatherStats != null ? StatRecord.Source.Father : StatRecord.Source.None;
                }

                record.Value = record.ParentValue + Utilities.SampleGaussian(Controller.Settings.mutationMean, Controller.Settings.mutationStdDev);
                record.Value = Mathf.Max(record.Value, 0.1f);
 
                toReturn.Data[stat] = record;
            }

            return toReturn;
        }
    }
}
