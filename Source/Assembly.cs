using HarmonyLib;
using System.Reflection;
using Verse;

namespace AnimalGenetics
{
    namespace Assembly
    {
        [StaticConstructorOnStartup]
        public static class AnimalGeneticsAssemblyLoader
        {
            static AnimalGeneticsAssemblyLoader()
            {
                var h = new Harmony("AnimalGenetics");
                h.PatchAll();
            }
        }

        [HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTick))]
        public class GrowUp
        {
            static public bool Prefix(Pawn_AgeTracker __instance, Pawn ___pawn)
            {
                while (!___pawn.Dead && !__instance.CurLifeStage.reproductive)
                {
                    Log.Warning("Aging pawn");
                    __instance.DebugMake1YearOlder();
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(Hediff_Pregnant), nameof(Hediff_Pregnant.Tick))]
        public class BabyOut
        {
            static public bool Prefix(Hediff_Pregnant __instance)
            {
                __instance.Severity = 1;
                return true;
            }
        }

    }
}