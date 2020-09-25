using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AnimalGenetics
{
    public static class DebugActions
    {
        [DebugAction("General", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void GiveBirthTogether()
        {
            var males = Find.Selector.SelectedPawns.Where(candidate => candidate.gender == Gender.Male);
            var females = Find.Selector.SelectedPawns.Where(candidate => candidate.gender == Gender.Female);

            if (males.Count() != 1 || females.Count() != 1)
                return;

            Hediff_Pregnant.DoBirthSpawn(females.First(), males.First());
            DebugActionsUtility.DustPuffFrom(females.First());
        }
    }

    /*
    [HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTick))]
    public class GrowUp
    {
        static public bool Prefix(Pawn_AgeTracker __instance, Pawn ___pawn)
        {
            if (___pawn.RaceProps.FleshType == FleshTypeDefOf.Normal)
            {
                while (!___pawn.Dead && !__instance.CurLifeStage.reproductive)
                {
                    Log.Warning("Aging pawn");
                    __instance.DebugMake1YearOlder();
                }
            }
            return false;
        }
    }
        */

    /*
    [HarmonyPatch(typeof(Hediff_Pregnant), nameof(Hediff_Pregnant.Tick))]
    public class BabyOut
    {
        static public bool Prefix(Hediff_Pregnant __instance)
        {
            __instance.Severity = 1;
            return true;
        }
    }
    */

    /*
    [HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.BodyResourceGrowthSpeed))]
    public static class FastGrowth
    {
        static public bool Prefix(ref float __result)
        {
            __result = 10000f;
            return false;
        }
    }
    */

    /*
    [HarmonyPatch(typeof(CompHatcher), nameof(CompHatcher.CompTick))]
    public static class FastHatch
    {
        static public bool Prefix(ref CompHatcher __instance)
        {
            __instance.Hatch();
            return false;
        }
    }
    */
}
