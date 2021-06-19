using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;
using System.Linq;

using AnimalGeneticsSettings = AnimalGenetics.Settings;
using GenesRecord = System.Collections.Generic.Dictionary<RimWorld.StatDef, AnimalGenetics.GeneRecord>;

namespace AnimalGenetics
{
    public static class Extensions
    {
        public static GeneticInformation AnimalGenetics(this Pawn pawn)
        {
            var comp = pawn.TryGetComp<GeneticInformation>();
            if (comp == null)
                Log.Warning("Could not get comp for pawn: " + pawn.ToStringSafe());
            return comp;
        }

        public static float GetGene(this Pawn pawn, StatDef stat)
        {
            var comp = pawn.TryGetComp<GeneticInformation>();
            if (comp == null)
                return 1.0f;
            return comp.GeneRecords[stat].Value;
        }
        public static GeneRecord GetGeneRecord(this Pawn pawn, StatDef stat)
        {
            return pawn.TryGetComp<GeneticInformation>()?.GeneRecords[stat];
        }
    }

    public static class ParentReferences
    {
        public class Record
        {
            public GeneticInformation ThisGeneticInformation;
            public GenesRecord Mother;
            public GenesRecord Father;
        }

        public static Record Pop()
        {
            return Data.Pop();
        }
        public static void Push(Record record)
        {
            Data.Push(record);
        }

        public static GeneticInformation GeneticInformation => Data
            .Select(record => record.ThisGeneticInformation).FirstOrDefault(record => record != null);

        public static GenesRecord MotherGeneticInformation => Data
            .Select(record => record.Mother).FirstOrDefault(record => record != null);

        public static GenesRecord FatherGeneticInformation => Data
            .Select(record => record.Father).FirstOrDefault(record => record != null);

        private static readonly Stack<Record> Data = new Stack<Record>();
    }
    
    public class FatherGeneticInformation : HediffComp
    {
        public GenesRecord GenesRecords => _fatherGeneRecords;

        public override void CompPostMake()
        {
            _fatherField = parent.GetType().GetField("father");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (_fatherGeneRecords != null || _fatherField == null)
                return;
            var father = _fatherField?.GetValue(parent) as Pawn;
            _fatherGeneRecords = father?.AnimalGenetics()?.GeneRecords;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            if (Scribe.EnterNode("animalGenetics"))
            {
                Scribe_Collections.Look(ref _fatherGeneRecords, "fatherGeneRecords", LookMode.Def, LookMode.Deep);
                Scribe.ExitNode();
            }
        }

        private GenesRecord _fatherGeneRecords;
        private FieldInfo _fatherField;
    }

    public class PawnGeneticInformation : GeneticInformation
    {
        private static List<Verse.WeakReference<PawnGeneticInformation>> _instances = new List<Verse.WeakReference<PawnGeneticInformation>>();
        public static IEnumerable<PawnGeneticInformation> Instances
        {
            get
            {
                _instances = _instances.Where(wr => wr.IsAlive).ToList();
                return _instances.Where(wr => wr.Target._geneRecords != null)
                    .Select(wr => wr.Target);
            }
        }
        public PawnGeneticInformation()
        {
            _instances.Add(new Verse.WeakReference<PawnGeneticInformation>(this));
        }
        public override void PostExposeData()
        {
            base.PostExposeData();

            if (Scribe.EnterNode("animalGenetics"))
            {
                Scribe_Collections.Look(ref _geneRecords, "geneRecords", LookMode.Def, LookMode.Deep);
                Scribe_Collections.Look(ref _motherGeneRecords, "motherGeneRecords", LookMode.Def, LookMode.Deep);
                Scribe_Collections.Look(ref _fatherGeneRecords, "fatherGeneRecords", LookMode.Def, LookMode.Deep);
                Scribe.ExitNode();
            }

            if (Scribe.mode == LoadSaveMode.Saving || _geneRecords != null || !(parent is Pawn pawn)) return;

            // Backwards Compat Load
            var animalGeneticsWorldComponent = Find.World.GetComponent<AnimalGenetics>();
            if (animalGeneticsWorldComponent.BackwardsCompatData.ContainsKey(pawn))
            {
                _geneRecords = new Dictionary<StatDef, GeneRecord>(animalGeneticsWorldComponent.BackwardsCompatData[pawn].Data);
            }
        }

        private void Generate()
        {
            if (!(parent is Pawn pawn))
                return;

            if (!Genes.EffectsThing(parent))
                return;

            // Last ditch effort, try to determine missing parent genes
            if (_motherGeneRecords == null)
                _motherGeneRecords = pawn.GetMother()?.AnimalGenetics()?.GeneRecords;
            if (_fatherGeneRecords == null)
                _fatherGeneRecords = pawn.GetFather()?.AnimalGenetics()?.GeneRecords;

            base.Generate();
        }

    }
    public class EggGeneticInformation : GeneticInformation
    {
        public override void PostExposeData()
        {
            base.PostExposeData();
            if (Scribe.EnterNode("animalGenetics"))
            {
                Scribe_Collections.Look(ref _geneRecords, "geneRecords", LookMode.Def, LookMode.Deep);
                Scribe_Collections.Look(ref _motherGeneRecords, "motherGeneRecords", LookMode.Def, LookMode.Deep);
                Scribe_Collections.Look(ref _fatherGeneRecords, "fatherGeneRecords", LookMode.Def, LookMode.Deep);
                Scribe.ExitNode();
            }
        }

        public override void CompTick()
        {
            if (_geneRecords != null)
                return;

            var comp = parent.TryGetComp<CompHatcher>();
            if (comp == null)
                return;

            if (_motherGeneRecords == null && comp.hatcheeParent != null)
                _motherGeneRecords = comp.hatcheeParent.AnimalGenetics().GeneRecords;

            if (_fatherGeneRecords == null && comp.otherParent != null)
                _fatherGeneRecords = comp.otherParent.AnimalGenetics().GeneRecords;

            // If we have gene records of both parents, let's generate now
            if (_motherGeneRecords != null && _fatherGeneRecords != null)
                Generate();
        }
    }

    public class GeneticInformation : ThingComp
    {
        protected GenesRecord _geneRecords;
        protected GenesRecord _motherGeneRecords;
        protected GenesRecord _fatherGeneRecords;

        public GenesRecord GeneRecords
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

            var geneticInformation = ParentReferences.GeneticInformation;
            if (geneticInformation != null)
            {
                _geneRecords = geneticInformation._geneRecords;
                _motherGeneRecords = geneticInformation._motherGeneRecords;
                _fatherGeneRecords = geneticInformation._fatherGeneRecords;
                return;
            }

            _motherGeneRecords = ParentReferences.MotherGeneticInformation;
            _fatherGeneRecords = ParentReferences.FatherGeneticInformation;

            // If we have gene records of both parents, let's generate now
            if (_motherGeneRecords != null && _fatherGeneRecords != null)
                Generate();
        }

        protected void Generate()
        {
            _geneRecords = new Dictionary<StatDef, GeneRecord>();
            var affectedStats = Constants.affectedStats;

            foreach (var stat in affectedStats)
            {
                float motherValue = _motherGeneRecords?[stat].Value ?? Mathf.Max(Rand.Gaussian(Settings.Core.mean, Settings.Core.stdDev), 0.1f);
                float fatherValue = _fatherGeneRecords?[stat].Value ?? Mathf.Max(Rand.Gaussian(Settings.Core.mean, Settings.Core.stdDev), 0.1f);

                float highValue = Math.Max(motherValue, fatherValue);
                float lowValue = Math.Min(motherValue, fatherValue);

                float? ToNullableFloat(bool nullify, float value) => nullify ? null : (float?)value;

                var record = new GeneRecord(ToNullableFloat(_motherGeneRecords == null, motherValue), ToNullableFloat(_fatherGeneRecords == null, fatherValue));
                record.ParentValue = Rand.Chance(Settings.Core.bestGeneChance) ? highValue : lowValue;

                if (record.ParentValue == motherValue)
                    record.Parent = _motherGeneRecords != null ? GeneRecord.Source.Mother : GeneRecord.Source.None;
                else
                    record.Parent = _fatherGeneRecords != null ? GeneRecord.Source.Father : GeneRecord.Source.None;

                record.Value = record.ParentValue + Rand.Gaussian(Settings.Core.mutationMean, Settings.Core.mutationStdDev);
                record.Value = Mathf.Max(record.Value, 0.1f);

                _geneRecords[stat] = record;
            }
        }
    }

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
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                BackwardsCompatData = new Dictionary<Thing, StatGroup>();
                _Things = new List<Thing>();
                _StatGroups = new List<StatGroup>();
                GC.Collect();
                foreach (PawnGeneticInformation gi in PawnGeneticInformation.Instances)
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
