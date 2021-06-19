using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluffyManager;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class ColonyManager
    {
        public static bool WasPatched { get; private set; }

        private static class Utilities
        {
            public const float Margin = 6f;
            private const float SmallIconSize = 16f;

            public static float DrawCogButton(Vector2 pos, float width, string label, Action action)
            {
                var thresholdRect = new Rect(
                    pos.x,
                    pos.y,
                    width,
                    SmallIconSize + 2 * Margin);

                var detailsWindowButtonRect = new Rect(
                    thresholdRect.xMax - SmallIconSize - Margin,
                    thresholdRect.y + (SmallIconSize + 2 * Margin - SmallIconSize) / 2f,
                    SmallIconSize,
                    SmallIconSize);

                Widgets.DrawHighlightIfMouseover(thresholdRect);

                Text.Anchor = TextAnchor.MiddleLeft;

                Rect labelRect = thresholdRect;
                labelRect.x += 6;
                Text.Font = GameFont.Small;
                Widgets.Label(labelRect, label);

                Text.Anchor = TextAnchor.UpperLeft;

                GUI.color = Mouse.IsOver(thresholdRect) ? GenUI.MouseoverColor : Color.white;
                GUI.DrawTexture(detailsWindowButtonRect, FluffyManager.Resources.Cog);
                GUI.color = Color.white;
                if (Widgets.ButtonInvisible(thresholdRect))
                {
                    action?.Invoke();
                }

                return SmallIconSize + 2 * Margin;
            }
        };

        private static List<KeyValuePair<Verse.WeakReference<object>, Data>> _managerJobToData = new List<KeyValuePair<Verse.WeakReference<object>, Data>>();

        private static Data GetDataOrCreate(object managerJob)
        {
            // Create an array of object-Data pairs
            var aliveObjects = _managerJobToData.Select(kv => new KeyValuePair<object, Data>(kv.Key.Target, kv.Value))
                                               .Where(kv => kv.Key != null).ToList();
            var matches = aliveObjects.Where(kv => kv.Key == managerJob);

            Data toRet;
            if (!matches.Any())
            {
                aliveObjects.Add(new KeyValuePair<object, Data>(managerJob, new Data()));
                toRet = aliveObjects.Last().Value;
            }
            else
            {
                toRet = matches.First().Value;
            }

            _managerJobToData = aliveObjects.Select(kv => new KeyValuePair<Verse.WeakReference<object>, Data>(new Verse.WeakReference<object>(kv.Key), kv.Value)).ToList();

            return toRet;
        }

        public static bool ModRunning
        {
            get { return LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "fluffy.colonymanager" || x.Name == "Colony Manager"); }
        }

        private class Data : IExposable
        {
            public bool UseWithTaming;
            public bool UseWithButchering;
            public Dictionary<StatDef, float> Values = new Dictionary<StatDef, float>();
            public Data()
            {
                foreach (var gene in Constants.affectedStats)
                    Values[gene] = 1.0f / Constants.affectedStats.Count();
            }
            public void ExposeData()
            {
                Scribe_Values.Look(ref UseWithTaming, "UseWithTaming");
                Scribe_Values.Look(ref UseWithButchering, "UseWithButchering");
                Scribe_Collections.Look(ref Values, "Values", LookMode.Def);
            }
        };

        class SettingsWindow : Window
        {
            readonly Data _data;
            public SettingsWindow(Data data)
            {
                _data = data;
                closeOnClickedOutside = true;
                draggable = true;
            }

            private float DoSlider(Listing_Standard listingStandard, StatDef gene)
            {
                var startHeight = listingStandard.CurHeight;

                var newValue = listingStandard.Slider(_data.Values[gene], 0.0001f, 0.999f);

                if (newValue == _data.Values[gene])
                    return listingStandard.CurHeight - startHeight;

                _data.Values[gene] = newValue;

                var others = _data.Values.Where(kv => kv.Key != gene).Sum(kv => kv.Value);
                var modifier = (1.0f - newValue) / others;

                foreach (var key in _data.Values.Keys.ToList().Where(key => key != gene))
                {
                    _data.Values[key] = Math.Min(Math.Max(modifier * _data.Values[key], 0.01f), 0.99f);
                }

                return listingStandard.CurHeight - startHeight;
            }

            public override Vector2 InitialSize => new Vector2(300f, 600);
            public override void DoWindowContents(Rect rect)
            {
                Widgets.DrawMenuSection(rect);

                var listingStandard = new Listing_Standard();
                listingStandard.Begin(rect.ContractedBy(Utilities.Margin));

                Text.Font = GameFont.Small;

                listingStandard.Label("AnimalGenetics.ColonyManager.Description".Translate());

                var position = new Vector2(rect.x, rect.y + listingStandard.CurHeight);
                var width = rect.width;

                Section(ref position, width - 2 * Utilities.Margin, DoCheckboxes, "AnimalGenetics.ColonyManager.UseWith".Translate());
                Section(ref position, width - 2 * Utilities.Margin, DoGenePreferences, "AnimalGenetics.ColonyManager.GeneImportance".Translate());

                listingStandard.End();
            }

            private float DoCheckboxes(Vector2 pos, float width)
            {
                //TooltipHandler.TipRegion(new Rect(pos.x, pos.y, width, 30), tooltip.Value);
                FluffyManager.Utilities.DrawToggle(new Rect(pos.x, pos.y, width, 30), "FM.Livestock.TamingHeader".Translate(), "AnimalGenetics.ColonyManager.TamingTooltip".Translate(), ref _data.UseWithTaming);
                FluffyManager.Utilities.DrawToggle(new Rect(pos.x, pos.y+30, width, 30), "FM.Livestock.ButcherHeader".Translate(), "AnimalGenetics.ColonyManager.ButcheringTooltip".Translate(), ref _data.UseWithButchering);
                return 60;
            }

            private float DoGenePreferences(Vector2 position, float width)
            {
                var rect = new Rect(position, new Vector2(width, 1000));

                var listingStandard = new Listing_Standard();
                listingStandard.Begin(rect);

                Text.Font = GameFont.Tiny;

                foreach (var gene in Constants.affectedStats)
                {
                    var label = Constants.GetLabel(gene);
                    var description = Constants.GetDescription(gene);

                    var highlightRect = listingStandard.Label(label, -1f, description);

                    highlightRect.height += DoSlider(listingStandard, gene);

                    Widgets.DrawHighlightIfMouseover(highlightRect);
                }

                listingStandard.End();

                return listingStandard.CurHeight;
            }
        };

        private static float GetGene(Pawn pawn, StatDef gene)
        {
            if (gene == AnimalGenetics.GatherYield && !Genes.Gatherable(pawn))
                return 0.0f;
            return Genes.GetGene(pawn, gene);
        }

        private static float CalculatePreferenceScore(Data data, Pawn pawn)
        {
            Log.Message("CalculatePreferenceScore for " + pawn.Name + ": " + Constants.affectedStats.Select(gene => GetGene(pawn, gene) * data.Values[gene]).Sum());
            return Constants.affectedStats.Select(gene => GetGene(pawn, gene) * data.Values[gene]).Sum();
        }

        private static MethodInfo _ManagerTab_Livestock_DrawTamingSection;
        private static MethodInfo _Widgets_Section_Section;

        private static void Section(ref Vector2 position, float width, Func<Vector2, float, float> drawerFunc, string header = null, int id = 0)
        {
            var parameters = new object[] { position, width, drawerFunc, header, id };
            _Widgets_Section_Section.Invoke(null, parameters);
            position = (Vector2)parameters[0];
        }

        public static void Patch(Harmony h)
        {
            WasPatched = true;

            try
            {
                _Widgets_Section_Section = typeof(Widgets_Section).GetMethod("Section");
                h.Patch(_Widgets_Section_Section,
                    prefix: new HarmonyMethod(typeof(ColonyManager), nameof(WidgetsSectionPrefix)));

                _ManagerTab_Livestock_DrawTamingSection = typeof(ManagerTab_Livestock).GetMethod("DrawTamingSection", BindingFlags.Instance | BindingFlags.NonPublic);
                h.Patch(AccessTools.Method(typeof(ManagerTab_Livestock), "DoContent"),
                    prefix: new HarmonyMethod(typeof(ColonyManager), nameof(DoContent_Prefix)),
                    postfix: new HarmonyMethod(typeof(ColonyManager), nameof(DoContent_Postfix)));

                h.Patch(AccessTools.Method(typeof(ManagerJob_Livestock), "DoButcherJobs"),
                    prefix: new HarmonyMethod(typeof(ColonyManager), nameof(DoButcherJobs_Prefix)));
                h.Patch(AccessTools.Method(typeof(ManagerJob_Livestock), "DoTamingJobs"),
                    prefix: new HarmonyMethod(typeof(ColonyManager), nameof(DoTamingJobs_Prefix)));
                h.Patch(AccessTools.Method(typeof(ManagerJob_Livestock), "ExposeData"),
                    postfix: new HarmonyMethod(typeof(ColonyManager), nameof(ExposeDataPatch)));

                JobsWrapper.Init();
            }
            catch (NullReferenceException)
            {
                Log.Message("Problem patching Colony Manager");

            }
        }

        public static void ExposeDataPatch(object __instance)
        {
            if (Scribe.EnterNode("animalGenetics"))
            {
                GetDataOrCreate(__instance).ExposeData();
                Scribe.ExitNode();
            }
        }

        private static object _CurrentJob;
        public static bool DoContent_Prefix(object ____selectedCurrent)
        {
            _CurrentJob = ____selectedCurrent;
            return true;
        }

        public static void DoContent_Postfix()
        {
            _CurrentJob = null;
        }

        public static bool DoButcherJobs_Prefix(object __instance, ref bool actionTaken)
        {
            var wrapper = new JobsWrapper(__instance);
            wrapper.DoButcherJobs(ref actionTaken);
            return false;
        }

        public static bool DoTamingJobs_Prefix(object __instance, ref bool actionTaken)
        {
            var wrapper = new JobsWrapper(__instance);
            wrapper.DoTamingJobs(ref actionTaken);
            return false;
        }
    
        // This is so we can insert a new section above the taming section
        public static bool WidgetsSectionPrefix(ref Vector2 position, float width, Func<Vector2, float, float> drawerFunc, MethodInfo __originalMethod)
        {
            if (drawerFunc.GetMethodInfo() != _ManagerTab_Livestock_DrawTamingSection) return true;

            var arguments = new object[] { position, width, (Func<Vector2, float, float>)DrawAnimalGeneticSection, "AnimalGenetics", 0 };
            __originalMethod.Invoke(null, arguments);
            position = (Vector2)arguments[0];
            return true;
        }

        private static float DrawAnimalGeneticSection(Vector2 pos, float width)
        {
            return Utilities.DrawCogButton(pos, width, "Configure Preferences", () => { Find.WindowStack.Add(new SettingsWindow(GetDataOrCreate(_CurrentJob))); });
        }

        class JobsWrapper
        {
            private static MethodInfo _tryRemoveDesignationMethod;
            public static void Init()
            {
                _tryRemoveDesignationMethod = typeof(ManagerJob_Livestock).GetMethod("TryRemoveDesignation",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (_tryRemoveDesignationMethod == null)
                    throw new NullReferenceException();
            }
            private readonly ManagerJob_Livestock _self;
            public JobsWrapper(object instance)
            {
                _self = (ManagerJob_Livestock)instance;
            }

            private bool ButcherExcess => _self.ButcherExcess;
            private bool ButcherTrained => _self.ButcherTrained;
            private bool ButcherPregnant => _self.ButcherPregnant;
            private bool ButcherBonded => _self.ButcherBonded;
            private bool TryTameMore => _self.TryTameMore;
            private Trigger_PawnKind Trigger => _self.Trigger;
            private Manager manager => _self.manager;
            private void AddDesignation(Pawn p, DesignationDef def) => _self.AddDesignation(p, def);
            private Area TameArea => _self.TameArea;
            private bool IsReachable(Thing target) => _self.IsReachable(target);
            private float Distance(Thing target, IntVec3 source) => _self.Distance(target, source);

            private bool TryRemoveDesignation(AgeAndSex ageSex, DesignationDef def) =>
                (bool)_tryRemoveDesignationMethod.Invoke(_self, new object[] {ageSex, def});

            private List<Designation> DesignationsOfOn(
                DesignationDef def,
                AgeAndSex ageSex) => _self.DesignationsOfOn(def, ageSex);

            public void DoButcherJobs(ref bool actionTaken)
            {
                if (!ButcherExcess) return;

#if DEBUG_LIFESTOCK
            Log.Message( "Doing butchery: " + Trigger.pawnKind.LabelCap );
#endif

                foreach (var ageSex in Utilities_Livestock.AgeSexArray)
                {
                    // too many animals?
                    var surplus = Trigger.pawnKind.GetTame(manager, ageSex).Count()
                                - DesignationsOfOn(DesignationDefOf.Slaughter, ageSex).Count
                                - Trigger.CountTargets[ageSex];

#if DEBUG_LIFESTOCK
                Log.Message( "Butchering " + ageSex + ", surplus" + surplus );
#endif

                    if (surplus > 0)
                    {
                        // should slaughter oldest adults, youngest juveniles.
                        var oldestFirst = ageSex == AgeAndSex.AdultFemale ||
                                          ageSex == AgeAndSex.AdultMale;

                        // get list of animals in correct sort order.
                        var animals = Trigger.pawnKind.GetTame(manager, ageSex)
                                             .Where(
                                                  p => manager.map.designationManager.DesignationOn(
                                                           p, DesignationDefOf.Slaughter) == null
                                                    && (ButcherTrained ||
                                                         !p.training.HasLearned(TrainableDefOf.Obedience))
                                                    && (ButcherPregnant || !p.VisiblyPregnant())
                                                    && (ButcherBonded || !p.BondedWithColonist()))
                                             .OrderBy(
                                                  p => (oldestFirst ? -1 : 1) * p.ageTracker.AgeBiologicalTicks)
                                             .ToList();

                        var preferenceData = GetDataOrCreate(_self);
                        if (preferenceData.UseWithButchering)
                            animals = animals.OrderBy(pawn => CalculatePreferenceScore(preferenceData, pawn)).ToList();

#if DEBUG_LIFESTOCK
                    Log.Message( "Tame animals: " + animals.Count );
#endif

                        for (var i = 0; i < surplus && i < animals.Count; i++)
                        {
#if DEBUG_LIFESTOCK
                        Log.Message( "Butchering " + animals[i].GetUniqueLoadID() );
#endif
                            AddDesignation(animals[i], DesignationDefOf.Slaughter);
                        }
                    }

                    // remove extra designations
                    while (surplus < 0)
                        if (TryRemoveDesignation(ageSex, DesignationDefOf.Slaughter))
                        {
#if DEBUG_LIFESTOCK
                        Log.Message( "Removed extra butchery designation" );
#endif
                            actionTaken = true;
                            surplus++;
                        }
                        else
                        {
                            break;
                        }
                }
            }
            public void DoTamingJobs(ref bool actionTaken)
            {
                if (!TryTameMore) return;

                foreach (var ageSex in Utilities_Livestock.AgeSexArray)
                {
                    // not enough animals?
                    var deficit = Trigger.CountTargets[ageSex]
                                - Trigger.pawnKind.GetTame(manager, ageSex).Count()
                                - DesignationsOfOn(DesignationDefOf.Tame, ageSex).Count;

#if DEBUG_LIFESTOCK
                Log.Message( "Taming " + ageSex + ", deficit: " + deficit );
#endif

                    if (deficit > 0)
                    {
                        // get the 'home' position
                        var position = manager.map.GetBaseCenter();

                        // get list of animals in sorted by youngest weighted to distance.
                        var animals = Trigger.pawnKind.GetWild(manager, ageSex)
                                             .Where(p => p != null &&
                                                         p.Spawned &&
                                                         manager.map.designationManager.DesignationOn(p) == null &&
                                                         (TameArea == null ||
                                                           TameArea.ActiveCells.Contains(p.Position)) &&
                                                         IsReachable(p)).ToList();

                        // skip if no animals available.
                        if (animals.Count == 0)
                            continue;

                        animals =
                            animals.OrderByDescending(p => p.ageTracker.AgeBiologicalTicks / Distance(p, position)).ToList();

                        var preferenceData = GetDataOrCreate(_self);
                        if (preferenceData.UseWithTaming)
                            animals = animals.OrderByDescending(pawn => CalculatePreferenceScore(preferenceData, pawn)).ToList();

#if DEBUG_LIFESTOCK
                    Log.Message( "Wild: " + animals.Count );
#endif

                        for (var i = 0; i < deficit && i < animals.Count; i++)
                        {
#if DEBUG_LIFESTOCK
                        Log.Message( "Adding taming designation: " + animals[i].GetUniqueLoadID() );
#endif
                            AddDesignation(animals[i], DesignationDefOf.Tame);
                        }
                    }

                    // remove extra designations
                    while (deficit < 0)
                        if (TryRemoveDesignation(ageSex, DesignationDefOf.Tame))
                        {
#if DEBUG_LIFESTOCK
                        Log.Message( "Removed extra taming designation" );
#endif
                            actionTaken = true;
                            deficit++;
                        }
                        else
                        {
                            break;
                        }
                }
            }
        }
    }
}
