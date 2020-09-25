using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AnimalGenetics
{
    public class StatRecord : IExposable
    {
        public StatRecord()
        {
        }
        public StatRecord(float? motherValue, float? fatherValue)
        {
            _motherValue = motherValue;
            _fatherValue = fatherValue;
            Value = 1.0f;
            ParentValue = 1.0f;
            Parent = StatRecord.Source.None;
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

    public class StatGroup : IExposable
    {
        public StatGroup()
        {
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Data, "values", LookMode.Def, LookMode.Deep);
        }
        public StatRecord GetFactor(StatDef stat)
        {
            if (!Data.ContainsKey(stat))
                return DefaultStat;
            return Data[stat];
        }

        public static StatRecord DefaultStat = new StatRecord(null, null);
        public Dictionary<StatDef, StatRecord> Data = new Dictionary<StatDef, StatRecord>();
    }

}
