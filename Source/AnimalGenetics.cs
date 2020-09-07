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
                    float baseValue = AnimalGenetics.GetData(mother).Get(stat);

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
        public float Get(StatDef stat)
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

        public StatGroup GetData(Pawn thing)
        {
            if (!_Data.ContainsKey(thing))
                _Data[thing] = new StatGroup(thing);
            return _Data[thing];
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref _Data, "data", LookMode.Reference, LookMode.Deep, ref _Things, ref _StatGroups);
        }

        Dictionary<Thing, StatGroup> _Data = new Dictionary<Thing, StatGroup>();
        List<Thing> _Things = new List<Thing>();
        List<StatGroup> _StatGroups = new List<StatGroup>();
    }

    [HarmonyPatch(typeof(StatExtension), nameof(StatExtension.GetStatValue))]
    static class PatchStats
    {
        static void Postfix(this Thing thing, ref float __result, StatDef stat, bool applyPostProcess)
        {
            if (!typeof(Pawn).IsInstanceOfType(thing))
                return;
            
            var AnimalGenetics = Find.World.GetComponent<AnimalGenetics>();

            if (AnimalGenetics == null)
                return;

            if (stat == StatDefOf.MoveSpeed)
            {
                var data = AnimalGenetics.GetData((Pawn)thing);

                if (data == null)
                    return;

                __result = __result * data.Get(stat);
            }


        }
    }
}
