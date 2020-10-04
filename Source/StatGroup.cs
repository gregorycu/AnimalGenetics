using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AnimalGenetics
{
    public class GeneRecord : IExposable
    {
        public GeneRecord()
        {
        }
        public GeneRecord(float? motherValue, float? fatherValue)
        {
            _motherValue = motherValue;
            _fatherValue = fatherValue;
            Value = 1.0f;
            ParentValue = 1.0f;
            Parent = Source.None;
        }

        public float Value;
        public float ParentValue;
        public enum Source
        {
            None, Mother, Father
        };
        public Source Parent;

        private float? _motherValue;
        private float? _fatherValue;

        public float? MotherValue
        {
            get
            {
                if (_motherValue != null)
                    return _motherValue;

                if (Parent == Source.Mother)
                    return ParentValue;

                return null;
            }
        }
        public float? FatherValue
        {
            get
            {
                if (_fatherValue != null)
                    return _fatherValue;

                if (Parent == Source.Father)
                    return ParentValue;

                return null;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref Value, "Value");
            Scribe_Values.Look(ref ParentValue, "ParentValue");
            Scribe_Values.Look(ref Parent, "Parent");
            Scribe_Values.Look(ref _motherValue, "MotherValue");
            Scribe_Values.Look(ref _fatherValue, "FatherValue");
        }
    };

    // Legacy Class - Still used for backwards compat
    public class StatGroup : IExposable
    {
        public StatGroup(GeneticInformation geneticInformation)
        {
            Data = new Dictionary<StatDef, GeneRecord>(geneticInformation.GeneRecords);
        }
        public StatGroup()
        {
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Data, "values", LookMode.Def, LookMode.Deep);
        }

        public static GeneRecord DefaultStat = new GeneRecord(null, null);
        public Dictionary<StatDef, GeneRecord> Data = new Dictionary<StatDef, GeneRecord>();
    }

}
