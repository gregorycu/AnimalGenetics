using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using System.Linq;

namespace AnimalGenetics
{
    namespace AnimalGeneticsAssemblyLoader
    {
        [StaticConstructorOnStartup]
        public static class AnimalGeneticsAssemblyLoader
        {
            static AnimalGeneticsAssemblyLoader()
            {
                var h = new HarmonyLib.Harmony("AnimalGenetics");
                h.PatchAll();

                var affectedStats = new List<StatDef> { StatDefOf.MoveSpeed };

                foreach (var stat in affectedStats)
                    stat.parts.Insert(0, new StatPart(stat));
            }
        }
    }

    public class StatGroup : IExposable
    {
        public StatGroup()
        {
        }

        public StatGroup(Pawn pawn)
        {
            var AnimalGenetics = Find.World.GetComponent<AnimalGenetics>();

            var father = pawn.GetFather();
            var mother = pawn.GetMother();

            var mutatedStats = GetMutatedStats();

            if (mother != null) // && father != null)
            {
                var keys = mutatedStats.Keys.ToList();
                foreach (var stat in keys)
                {
                    //float baseValue = (AnimalGenetics.GetData(mother).Get(stat) + AnimalGenetics.GetData(father).Get(stat)) / 2.0f;
                    float baseValue = AnimalGenetics.GetFactor(mother, stat);

                    Log.Warning("Has mother with " + baseValue.ToString() + " multiplying with " + mutatedStats[stat].ToString());
    
                    mutatedStats[stat] = baseValue * mutatedStats[stat];
                }
            }
            _Data = mutatedStats;
        }

        Dictionary<StatDef, float> GetMutatedStats()
        {
            return new Dictionary<StatDef, float>
            {
                { StatDefOf.MoveSpeed,  _Random.Next(9000, 11000) / 10000.0f }, 
            };
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref _Data, "values", LookMode.Def, LookMode.Value);
        }
        public float GetFactor(StatDef stat)
        {
            if (!_Data.ContainsKey(stat))
                return 1.0f;
            return _Data[stat];
        }

        Dictionary<StatDef, float> _Data = new Dictionary<StatDef, float>();

        static Random _Random = new Random();
    }

    public class AnimalGenetics : WorldComponent
    {
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
                _Data[pawn] = new StatGroup(pawn);
            return _Data[pawn];
        }

        public float GetFactor(Pawn pawn, StatDef stat)
        {
            return GetData(pawn).GetFactor(stat);
        }
    }

    // Token: 0x0200108A RID: 4234
    public class StatPart : RimWorld.StatPart
    {
        public StatPart(StatDef statDef)
        {
            _StatDef = statDef;
            priority = 1.1f;
        }

        // Token: 0x06006665 RID: 26213 RVA: 0x0023A4F7 File Offset: 0x002386F7
        public override void TransformValue(StatRequest req, ref float val)
        {
            var factor = GetFactor(req);
            if (factor != null)
                val = val * (float)factor;
        }

        // Token: 0x06006666 RID: 26214 RVA: 0x0001A1D9 File Offset: 0x000183D9
        public override string ExplanationPart(StatRequest req)
        {
            var factor = GetFactor(req);

            if (factor == null)
                return null;

            return "Genetics: x" + GenText.ToStringPercent((float)factor);
        }

        float? GetFactor(StatRequest req)
        {
            if (!req.HasThing)
                return null;

            Pawn pawn = req.Thing as Pawn;

            if (pawn == null)
                return null;

            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, _StatDef);
        }

        StatDef _StatDef;
    }


}
