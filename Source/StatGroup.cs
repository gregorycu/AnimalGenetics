using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AnimalGenetics
{
    public class StatRecord : IExposable
    {
        public float Value;
        public float ParentValue;
        public enum Source
        {
            None, Mother, Father
        };
        public Source Parent;

        public void ExposeData()
        {
            Scribe_Values.Look(ref Value, "Value");
            Scribe_Values.Look(ref ParentValue, "ParentValue");
            Scribe_Values.Look(ref Parent, "Parent");
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

        public static StatRecord DefaultStat = new StatRecord { Value = 1.0f, ParentValue = 1.0f, Parent = StatRecord.Source.None };

        public Dictionary<StatDef, StatRecord> Data = new Dictionary<StatDef, StatRecord>();
    }

}
