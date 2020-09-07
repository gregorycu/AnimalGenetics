using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AnimalGenetics
{
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
                { StatDefOf.MaxHitPoints,  _Random.Next(9000, 11000) / 10000.0f },
                { StatDefOf.LeatherAmount,  _Random.Next(9000, 11000) / 10000.0f },
                { StatDefOf.MeatAmount,  _Random.Next(9000, 11000) / 10000.0f },
                { StatDefOf.CarryingCapacity,  _Random.Next(9000, 11000) / 10000.0f }
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

}
