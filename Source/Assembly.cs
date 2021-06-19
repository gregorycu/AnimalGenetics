using HarmonyLib;
using System.Collections.Generic;
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
            private static List<PawnColumnDef> _DefaultAnimalsPawnTableDefColumns;
            private static List<PawnColumnDef> _DefaultWildlifePawnTableDefColumns;

            public static Type typeAlphaAnimals;
            public static List<Type> gatherableTypes;
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
                        Log.Error(stat + " is broken");
                    }
                }

                gatherableTypes = new List<Type>()
                {
                    typeof(CompShearable),
                    typeof(CompMilkable)
                };

                // Compatibility patches
                try
                {
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "sarg.alphaanimals" || x.Name == "Alpha Animals"))
                    {
                        Log.Message("Animal Genetics : Alpha Animals is loaded - Patching");
                        h.Patch(AccessTools.Method(AccessTools.TypeByName("AlphaBehavioursAndEvents.CompAnimalProduct"), "get_ResourceAmount"),
                            postfix: new HarmonyMethod(typeof(CompatibilityPatches), nameof(CompatibilityPatches.AlphaAnimals_get_ResourceAmount_Patch)));
                        gatherableTypes.Add(AccessTools.TypeByName("AlphaBehavioursAndEvents.CompAnimalProduct"));
                    }
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "CETeam.CombatExtended" || x.Name == "Combat Extended"))
                    {
                        //gatherableTypes.Append(AccessTools.TypeByName("CombatExtended.CompMilkableRenameable")); //they all use shearable
                        gatherableTypes.Add(AccessTools.TypeByName("CombatExtended.CompShearableRenameable"));
                    }

                    if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "rim.job.world"))
                    {
                        Log.Message("Patched RJW");
                        h.Patch(AccessTools.Method(AccessTools.TypeByName("rjw.Hediff_BasePregnancy"), "GenerateBabies"),
                            prefix: new HarmonyMethod(typeof(CompatibilityPatches), nameof(CompatibilityPatches.RJW_GenerateBabies_Prefix)),
                            postfix: new HarmonyMethod(typeof(CompatibilityPatches), nameof(CompatibilityPatches.RJW_GenerateBabies_Postfix)));
                    }
                }
                catch { }

                if (ColonyManager.ModRunning && Settings.Integration.ColonyManagerIntegration)
                    ColonyManager.Patch(h);

                _DefaultAnimalsPawnTableDefColumns = new List<PawnColumnDef>(PawnTableDefOf.Animals.columns);
                _DefaultWildlifePawnTableDefColumns = new List<PawnColumnDef>(PawnTableDefOf.Wildlife.columns);

                var placeholderPosition = MainTabWindow_AnimalGenetics.PawnTableDefs.Genetics.columns.FindIndex(def => def.defName == "AnimalGenetics_Placeholder");
                MainTabWindow_AnimalGenetics.PawnTableDefs.Genetics.columns.RemoveAt(placeholderPosition);
                MainTabWindow_AnimalGenetics.PawnTableDefs.Genetics.columns.InsertRange(placeholderPosition, PawnTableColumnsDefOf.Genetics.columns);

                PatchUI();
            }

            public static class PatchState
            {
                public static bool PatchedGenesInAnimalsTab;
                public static bool PatchedGenesInWildlifeTab = false;
            }

            public static void PatchUI()
            {
                if (PatchState.PatchedGenesInAnimalsTab != Settings.UI.showGenesInAnimalsTab)
                {
                    PawnTableDefOf.Animals.columns = new List<PawnColumnDef>(_DefaultAnimalsPawnTableDefColumns);
                    if (Settings.UI.showGenesInAnimalsTab)
                        PawnTableDefOf.Animals.columns.AddRange(PawnTableColumnsDefOf.Genetics.columns);
                    PatchState.PatchedGenesInAnimalsTab = Settings.UI.showGenesInAnimalsTab;
                }
                if (PatchState.PatchedGenesInWildlifeTab != Settings.UI.showGenesInWildlifeTab)
                {
                    PawnTableDefOf.Wildlife.columns = new List<PawnColumnDef>(_DefaultWildlifePawnTableDefColumns);
                    if (Settings.UI.showGenesInWildlifeTab)
                        PawnTableDefOf.Wildlife.columns.AddRange(PawnTableColumnsDefOf.Genetics.columns);
                    PatchState.PatchedGenesInWildlifeTab = Settings.UI.showGenesInWildlifeTab;
                }

                var mainButton = DefDatabase<MainButtonDef>.GetNamed("AnimalGenetics");
                mainButton.buttonVisible = Settings.UI.showGeneticsTab;
            }
        }

        [HarmonyPatch(typeof(Hediff_Pregnant), nameof(Hediff_Pregnant.DoBirthSpawn))]
        public static class DoBirthSpawn_Patch
        {
            public static void Prefix(Pawn mother, Pawn father)
            {
                var motherGenes = mother?.AnimalGenetics()?.GeneRecords;
                var fatherGenes = father?.AnimalGenetics()?.GeneRecords;

                if (fatherGenes == null && mother != null)
                {
                    var fatherGeneticInformationComp = mother.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant)
                        .TryGetComp<FatherGeneticInformation>();
                    fatherGenes = fatherGeneticInformationComp?.GenesRecords;
                }

                ParentReferences.Push(new ParentReferences.Record {Mother = motherGenes, Father = fatherGenes });
            }

            public static void Postfix()
            {
                ParentReferences.Pop();
            }
        }

        [HarmonyPatch(typeof(CompHatcher), nameof(CompHatcher.Hatch))]
        public static class CompHatcher_Hatch_Patch
        {
            public static void Prefix(ThingWithComps ___parent)
            {
                var geneticInformation = ___parent.TryGetComp<GeneticInformation>();
                ParentReferences.Push(new ParentReferences.Record { ThisGeneticInformation = geneticInformation });
            }

            public static void Postfix()
            {
                ParentReferences.Pop();
            }
        }

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

                __result = (int)(__result * Genes.GetGene((Pawn)__instance.parent, AnimalGenetics.GatherYield));
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

        // Compatibility Patches
        public static class CompatibilityPatches
        {
            
            public static void AlphaAnimals_get_ResourceAmount_Patch(ref int __result, CompHasGatherableBodyResource __instance)
            {
                __result = (int)(__result * Genes.GetGene((Pawn)__instance.parent, AnimalGenetics.GatherYield));
            }

            public static void RJW_GenerateBabies_Prefix(Pawn ___pawn, Pawn ___father)
            {
                ParentReferences.Push(new ParentReferences.Record
                {
                    Mother =  ___pawn?.AnimalGenetics()?.GeneRecords,
                    Father = ___father?.AnimalGenetics()?.GeneRecords,
                });
            }

            public static void RJW_GenerateBabies_Postfix(Pawn ___pawn)
            {
                ParentReferences.Pop();
            }
        }
    }
}
