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
            var comp = pawn.TryGetComp<BaseGeneticInformation>().GeneticInformation;
            if (comp == null)
                Log.Warning("Could not get comp for pawn: " + pawn.ToStringSafe());
            return comp;
        }

        public static float GetGene(this Pawn pawn, StatDef stat)
        {
            var comp = pawn.AnimalGenetics();
            if (comp == null)
                return 1.0f;
            return comp.GeneRecords[stat].Value;
        }
        public static GeneRecord GetGeneRecord(this Pawn pawn, StatDef stat)
        {
            return pawn.AnimalGenetics()?.GeneRecords[stat];
        }
    }

    public static class ParentReferences
    {
        public class Record
        {
            public GeneticInformation This;
            public GeneticInformation Mother;
            public GeneticInformation Father;
        }

        public static Record Pop()
        {
            return Data.Pop();
        }
        public static void Push(Record record)
        {
            Data.Push(record);
        }

        public static GeneticInformation ThisGeneticInformation => Data
            .Select(record => record.This).FirstOrDefault(record => record != null);

        public static GeneticInformation MotherGeneticInformation => Data
            .Select(record => record.Mother).FirstOrDefault(record => record != null);

        public static GeneticInformation FatherGeneticInformation => Data
            .Select(record => record.Father).FirstOrDefault(record => record != null);

        private static readonly Stack<Record> Data = new Stack<Record>();
    }
    
    public class FatherGeneticInformation : HediffComp
    {
        public GeneticInformation GeneticInformation => _geneticInformation;

        public override void CompPostMake()
        {
            _fatherField = parent.GetType().GetField("father");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (_geneticInformation != null || _fatherField == null)
                return;
            var father = _fatherField?.GetValue(parent) as Pawn;
            _geneticInformation = father?.AnimalGenetics();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            if (Scribe.EnterNode("animalGenetics"))
            {
                Scribe_References.Look(ref _geneticInformation, "fatherGeneRecords");
                Scribe.ExitNode();
            }
        }

        private GeneticInformation _geneticInformation;
        private FieldInfo _fatherField;
    }

    public class PawnGeneticInformation : BaseGeneticInformation
    {
    }

    public class EggGeneticInformation : BaseGeneticInformation
    {
        public override void CompTick()
        {
            var comp = parent.TryGetComp<CompHatcher>();
            if (comp == null)
                return;

            if (GeneticInformation.Mother == null && comp.hatcheeParent != null)
                GeneticInformation.Mother = comp.hatcheeParent.AnimalGenetics();

            if (GeneticInformation.Father == null && comp.otherParent != null)
                GeneticInformation.Father = comp.otherParent.AnimalGenetics();
        }
    }

    public class GeneticInformation : ILoadReferenceable, IExposable
    {
        private static List<Verse.WeakReference<GeneticInformation>> _instances = new List<Verse.WeakReference<GeneticInformation>>();

        private static IEnumerable<GeneticInformation> Instances
        {
            get
            {
                _instances = _instances.Where(wr => wr.IsAlive).ToList();
                return _instances.Where(wr => wr.Target != null)
                    .Select(wr => wr.Target);
            }
        }

        public GeneticInformation()
        {
            _instances.Add(new Verse.WeakReference<GeneticInformation>(this));
            Mother = ParentReferences.MotherGeneticInformation;
            Father = ParentReferences.FatherGeneticInformation;
        }

        public GeneticInformation Mother;
        public GeneticInformation Father;

        public GenesRecord GeneRecords
        {
            get
            {
                if (_geneRecords == null)
                    _geneRecords = GeneticCalculator.GenerateGenesRecord(Mother, Father);
                return _geneRecords;
            }
        }

        private GenesRecord _geneRecords;

        void IExposable.ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
                ((ILoadReferenceable)this).GetUniqueLoadID();

            Scribe_Values.Look(ref _loadId, "loadID", 0, true);

            Scribe_References.Look(ref Mother, "motherLoadID");
            Scribe_References.Look(ref Father, "fatherLoadID");

            Scribe_Collections.Look(ref _geneRecords, "genes");
        }

        private static int _nextLoadId = 1;

        public static void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars)
                _instances.Clear();

            var instances = Instances.ToList();
            Scribe_Collections.Look(ref instances, "geneticInformation", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                _nextLoadId = instances.Count > 0 ? instances.Max(e => e._loadId) + 1 : 1;
        }

        private int _loadId;
        string ILoadReferenceable.GetUniqueLoadID()
        {
            if (_loadId == 0)
                _loadId = _nextLoadId++;
            return _loadId.ToString();
        }
    }

    public static class GeneticCalculator
    {
        public static GenesRecord GenerateGenesRecord(GeneticInformation mother, GeneticInformation father)
        {
            var result = new Dictionary<StatDef, GeneRecord>();
            var affectedStats = Constants.affectedStats;

            foreach (var stat in affectedStats)
            {
                var motherStat = mother?.GeneRecords?[stat];
                var fatherStat = father?.GeneRecords?[stat];

                float motherValue = motherStat?.Value ??
                                    Mathf.Max(Rand.Gaussian(Settings.Core.mean, Settings.Core.stdDev), 0.1f);
                float fatherValue = fatherStat?.Value ??
                                    Mathf.Max(Rand.Gaussian(Settings.Core.mean, Settings.Core.stdDev), 0.1f);

                float highValue = Math.Max(motherValue, fatherValue);
                float lowValue = Math.Min(motherValue, fatherValue);

                var record = new GeneRecord();

                var parentValue = Rand.Chance(Settings.Core.bestGeneChance) ? highValue : lowValue;
                var delta = Rand.Gaussian(Settings.Core.mutationMean, Settings.Core.mutationStdDev);

                if (parentValue == motherValue)
                    record.Parent = motherStat != null ? GeneRecord.Source.Mother : GeneRecord.Source.None;
                else
                    record.Parent = fatherStat != null ? GeneRecord.Source.Father : GeneRecord.Source.None;

                record.Value = parentValue + delta;

                result[stat] = record;
            }

            return result;
        }
    }

    public class BaseGeneticInformation : ThingComp
    {
        public GeneticInformation GeneticInformation;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            GeneticInformation = ParentReferences.ThisGeneticInformation ?? new GeneticInformation();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            if (Scribe.EnterNode("animalGenetics"))
            {
                Scribe_References.Look(ref GeneticInformation, "geneticInformation");
                Scribe.ExitNode();
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
            GeneticInformation.ExposeData();

            if (Scribe.EnterNode("settings"))
            {
                Settings.ExposeData();
                Scribe.ExitNode();
            }
        }
    }
}
