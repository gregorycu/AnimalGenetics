using HarmonyLib;
using System.Reflection;
using System;
using Verse;
using RimWorld;


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

                DefDatabase<StatDef>.Add(AnimalGenetics.Damage);
                DefDatabase<StatDef>.Add(AnimalGenetics.Health);
                DefDatabase<StatDef>.Add(AnimalGenetics.GatherYield);
            }
        }

        /*[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTick))]
        public class GrowUp
        {
            static public bool Prefix(Pawn_AgeTracker __instance, Pawn ___pawn)
            {
                if (___pawn.RaceProps.FleshType == FleshTypeDefOf.Normal) {
                    while (!___pawn.Dead && !__instance.CurLifeStage.reproductive)
                    {
                        Log.Warning("Aging pawn");
                        __instance.DebugMake1YearOlder();
                    }
                }
                return false;
            }
        }*/

        /*[HarmonyPatch(typeof(Hediff_Pregnant), nameof(Hediff_Pregnant.Tick))]
        public class BabyOut
        {
            static public bool Prefix(Hediff_Pregnant __instance)
            {
                __instance.Severity = 1;
                return true;
            }
        }*/

        [HarmonyPatch(typeof(MassUtility), nameof(MassUtility.Capacity))]
        public static class MassUtility_Capacity_Patch
        {
            static public void Postfix(ref float __result, Pawn __0)
            {
                __result = __result * Genes.GetGene(__0, StatDefOf.CarryingCapacity);
            }
        }



        [HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.GetDamageFactorFor), new[] { typeof(Tool), typeof(Pawn), typeof(HediffComp_VerbGiver) })]
        public static class VerbProperties_GetDanageFactorFor_Patch
        {
            static public void Postfix(ref float __result, Pawn __1)
            {
                if (__1.RaceProps.Animal) {
                    __result = __result * Genes.GetGene(__1, AnimalGenetics.Damage);
                }
            }
        }

        [HarmonyPatch(typeof(CompMilkable), "get_ResourceAmount")]
        public static class PatchCompMilkable_ResourceAmount
        {
            static public void Postfix(ref int __result, CompMilkable __instance)
            {
                Pawn pawn = __instance.parent as Pawn;
                if (pawn == null || !pawn.RaceProps.Animal)
                    return;

                __result =(int)(__result * Genes.GetGene(pawn, AnimalGenetics.GatherYield));
            }
        }

        [HarmonyPatch(typeof(CompShearable), "get_ResourceAmount")]
        public static class PatchCompShearable_ResourceAmount
        {
            static public void Postfix(ref int __result, CompShearable __instance)
            {
                Pawn pawn = __instance.parent as Pawn;
                if (pawn == null || !pawn.RaceProps.Animal)
                    return;

                __result = (int)(__result * Genes.GetGene(pawn, AnimalGenetics.GatherYield));
            }
        }

        [HarmonyPatch(typeof(Pawn), "get_HealthScale")]
        public static class Pawn_HealthScale_Patch
        {
            static public void Postfix(ref float __result, ref Pawn __instance)
            {
                if (__instance.RaceProps.Animal)
                {
                    __result = __result * Genes.GetGene(__instance, AnimalGenetics.Health);
                }
            }
        }
    }
}
