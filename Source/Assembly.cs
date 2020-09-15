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

                var affectedStats = Constants.affectedStatsToInsert;
                foreach (var stat in affectedStats)
                {
                    try
                    {
                        if (stat.parts != null)
                            stat.parts.Insert(0, new StatPart(stat));
                    }
                    catch
                    {
                        Log.Error(stat.ToString() + " is broken");
                    }
                }
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

        /*[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.BodyResourceGrowthSpeed))]
        public static class FastGrowth
        {
            static public bool Prefix(ref float __result)
            {
                __result = 10000f;
                return false;
            }
        }*/

        /*[HarmonyPatch(typeof(CompHatcher), nameof(CompHatcher.CompTick))]
        public static class FastHatch
        {
            static public bool Prefix(ref CompHatcher __instance)
            {
                __instance.Hatch();
                return false;
            }
        }*/

        [HarmonyPatch(typeof(MassUtility), nameof(MassUtility.Capacity))]
        public static class MassUtility_Capacity_Patch
        {
            static public void Postfix(ref float __result, Pawn __0)
            {
                if (!Genes.EffectsThing(__0))
                    return;

                __result = __result * Genes.GetGene(__0, StatDefOf.CarryingCapacity);
            }
        }

        [HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.GetDamageFactorFor), new[] { typeof(Tool), typeof(Pawn), typeof(HediffComp_VerbGiver) })]
        public static class VerbProperties_GetDanageFactorFor_Patch
        {
            static public void Postfix(ref float __result, Pawn __1)
            {
                if (!Genes.EffectsThing(__1))
                    return;

                __result = __result * Genes.GetGene(__1, AnimalGenetics.Damage);
            }
        }

        [HarmonyPatch(typeof(CompMilkable), "get_ResourceAmount")]
        public static class PatchCompMilkable_ResourceAmount
        {
            static public void Postfix(ref int __result, CompMilkable __instance)
            {
                if (!Genes.EffectsThing(__instance.parent))
                    return;

                __result =(int)(__result * Genes.GetGene((Pawn)__instance.parent, AnimalGenetics.GatherYield));
            }
        }

        [HarmonyPatch(typeof(CompShearable), "get_ResourceAmount")]
        public static class PatchCompShearable_ResourceAmount
        {
            static public void Postfix(ref int __result, CompShearable __instance)
            {
                if (!Genes.EffectsThing(__instance.parent))
                    return;

                __result = (int)(__result * Genes.GetGene((Pawn)(__instance.parent), AnimalGenetics.GatherYield));
            }
        }

        [HarmonyPatch(typeof(Pawn), "get_HealthScale")]
        public static class Pawn_HealthScale_Patch
        {
            static public void Postfix(ref float __result, ref Pawn __instance)
            {
                if (!Genes.EffectsThing(__instance))
                    return;

                __result = __result * Genes.GetGene(__instance, AnimalGenetics.Health);
            }
        }
    }
}
