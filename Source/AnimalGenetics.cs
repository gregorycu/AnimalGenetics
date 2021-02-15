using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;
using System.Linq;
using AnimalGenetics;
using Math = System.Math;
using Multiplayer.API;

using AnimalGeneticsSettings = AnimalGenetics.Settings;

namespace AnimalGenetics
{
    public static class Extensions
    {
        public static GeneticInformation AnimalGenetics(this Pawn pawn)
        {
            return pawn.TryGetComp<GeneticInformation>();
        }

        public static float GetGene(this Pawn pawn, StatDef stat)
        {
            return pawn.AnimalGenetics().GeneRecords[stat].Value;
        }
    }

    public static class ParentReferences
    {
        public class Record
        {
            public Pawn mother;
            public Pawn father;
        }

        public static Record Pop()
        {
            return _Data.Pop();
        }
        public static void Push(Record record)
        {
            _Data.Push(record);
        }

        public static Record Peek()
        {
            if (_Data.Count == 0)
                return null;
            return _Data.Peek();
        }

        static Stack<Record> _Data = new Stack<Record>();
    }

    public class GeneticInformation : ThingComp
    {
        static List<WeakReference<GeneticInformation>> _Instances = new List<WeakReference<GeneticInformation>>();
        public static IEnumerable<GeneticInformation> Instances
        {
            get
            {
                _Instances = _Instances.Where((WeakReference<GeneticInformation> wr) => wr.IsAlive).ToList();
                return _Instances.Where((WeakReference<GeneticInformation> wr) => wr.Target._geneRecords != null)
                                 .Select((WeakReference<GeneticInformation> wr) => wr.Target);
            }
        }
        public GeneticInformation()
        {
            _Instances.Add(new WeakReference<GeneticInformation>(this));
        }

        Dictionary<StatDef, GeneRecord> _geneRecords = null;
        public Dictionary<StatDef, GeneRecord> GeneRecords
        {
            get
            {
                if (_geneRecords == null)
                    Generate();
                return _geneRecords;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            // If we have a record of the parent data, generate stats now
            if (ParentReferences.Peek() != null)
                Generate(ParentReferences.Peek().mother, ParentReferences.Peek().father);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            if (!(parent is Pawn pawn))
                return;

            if (Scribe.EnterNode("animalGenetics"))
            {
                Scribe_Collections.Look(ref _geneRecords, "geneRecords", LookMode.Def, LookMode.Deep);
                Scribe.ExitNode();
            }

            // Backwards Compat Load
            if (Verse.Scribe.mode != LoadSaveMode.Saving)
            {
                if (_geneRecords == null)
                {
                    var animalGeneticsWorldComponent = Find.World.GetComponent<AnimalGenetics>();
                    if (animalGeneticsWorldComponent.BackwardsCompatData.ContainsKey(pawn))
                    {
                        _geneRecords = new Dictionary<StatDef, GeneRecord>(animalGeneticsWorldComponent.BackwardsCompatData[pawn].Data);
                    }
                }
            }
        }

        public void Generate(Pawn mother = null, Pawn father = null)
        {
            int RandSeed = 0;
            if (MP.IsInMultiplayer)
            {
                Rand.PushState();
                RandSeed = Rand.RangeInclusive(-100000, 100000);
                Rand.PopState();
            }
            if (!(parent is Pawn pawn))
                return;

            if (!Genes.EffectsThing(parent))
                return;

            _geneRecords = new Dictionary<StatDef, GeneRecord>();

            if (mother == null)
                mother = pawn.GetMother();

            if (father == null)
                father = pawn.GetFather();

            var motherStats = mother?.AnimalGenetics().GeneRecords;
            var fatherStats = father?.AnimalGenetics().GeneRecords;

            var affectedStats = Constants.affectedStats;

            foreach (var stat in affectedStats)
            {
                if (MP.IsInMultiplayer) Rand.PushState(RandSeed);
                float motherValue = motherStats != null ? motherStats[stat].Value : Mathf.Max(Verse.Rand.Gaussian(Settings.Core.mean, Settings.Core.stdDev), 0.1f);
                float fatherValue = fatherStats != null ? fatherStats[stat].Value : Mathf.Max(Verse.Rand.Gaussian(Settings.Core.mean, Settings.Core.stdDev), 0.1f);

                float highValue = Math.Max(motherValue, fatherValue);
                float lowValue = Math.Min(motherValue, fatherValue);

                float? ToNullableFloat(bool nullify, float value) => nullify ? null : (float?)value;

                var record = new GeneRecord(ToNullableFloat(mother == null, motherValue), ToNullableFloat(father == null, fatherValue));
                record.ParentValue = Verse.Rand.Chance(Settings.Core.bestGeneChance) ? highValue : lowValue;

                if (record.ParentValue == motherValue)
                    record.Parent = motherStats != null ? GeneRecord.Source.Mother : GeneRecord.Source.None;
                else
                    record.Parent = fatherStats != null ? GeneRecord.Source.Father : GeneRecord.Source.None;

                record.Value = record.ParentValue + Verse.Rand.Gaussian(Settings.Core.mutationMean, Settings.Core.mutationStdDev);
                record.Value = Mathf.Max(record.Value, 0.1f);

                _geneRecords[stat] = record;
                if (MP.IsInMultiplayer)
                {
                    Rand.PopState();
                    RandSeed += 1;
                }
            }
        }
    };

    public class AnimalGenetics : WorldComponent
    {
        public CoreSettings Settings = new CoreSettings();

        public static StatDef GatherYield = new StatDef { defName = "GatherYield", description = "AG.GatherYieldDesc".Translate(), alwaysHide = true };
        public static StatDef Damage = new StatDef { defName = "Damage", description = "AG.DamageDesc".Translate(),alwaysHide = true };
        public static StatDef Health = new StatDef { defName = "Health", description = "AG.HealthDesc".Translate(), alwaysHide = true };

        public AnimalGenetics(World world) : base(world)
        {
            Settings = (CoreSettings)AnimalGeneticsSettings.InitialCore.Clone();
        }

        public override void ExposeData()
        {
            if (Verse.Scribe.mode == LoadSaveMode.Saving)
            {
                BackwardsCompatData = new Dictionary<Thing, StatGroup>();
                _Things = new List<Thing>();
                _StatGroups = new List<StatGroup>();
                System.GC.Collect();
                foreach (GeneticInformation gi in GeneticInformation.Instances)
                    BackwardsCompatData[gi.parent] = new StatGroup(gi);
            }

            Scribe_Collections.Look(ref BackwardsCompatData, "data", LookMode.Reference, LookMode.Deep, ref _Things, ref _StatGroups);

            if (Scribe.EnterNode("settings"))
            {
                Settings.ExposeData();
                Scribe.ExitNode();
            }
        }

        public Dictionary<Thing, StatGroup> BackwardsCompatData = new Dictionary<Thing, StatGroup>();
        List<Thing> _Things = new List<Thing>();
        List<StatGroup> _StatGroups = new List<StatGroup>();
    }
}
