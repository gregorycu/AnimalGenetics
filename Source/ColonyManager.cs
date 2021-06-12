using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class ColonyManager
    {
        public static bool WasPatched { get; private set; } = false;

        public class Utilities
        {
            public const float Margin = 6f;
            public const float SmallIconSize = 16f;
            public const float SliderHeight = 20f;

            public static void Patch(Harmony h)
            {
                var parameters = typeof(Utilities).GetMethod("DrawToggle")?.GetParameters().Types();
                if (parameters == null)
                    return;
                var drawToggleMethod = AccessTools.TypeByName("FluffyManager.Utilities")?.GetMethod("DrawToggle", parameters);
                _DrawToggle = drawToggleMethod;
            }

            static MethodInfo _DrawToggle;
            public static void DrawToggle(Rect rect, string label, TipSignal tooltip, ref bool checkOn,
                               bool expensive = false, float size = SmallIconSize, float margin = Margin,
                               GameFont font = GameFont.Small, bool wrap = true)
            {
                var parameters = new object[] { rect, label, tooltip, checkOn, expensive, size, margin, font, wrap };
                _DrawToggle?.Invoke(null, parameters);
                checkOn = (bool)parameters[3];
            }

            public static float DrawCogButton(Vector2 pos, float width, string label, System.Action action)
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
                GUI.DrawTexture(detailsWindowButtonRect, Cog);
                GUI.color = Color.white;
                if (Widgets.ButtonInvisible(thresholdRect))
                {
                    action?.Invoke();
                }

                return SmallIconSize + 2 * Margin;
            }

        };

        public static List<KeyValuePair<Verse.WeakReference<object>, Data>> ManagerJobToData = new List<KeyValuePair<Verse.WeakReference<object>, Data>>();

        public static Data GetDataOrCreate(object managerJob)
        {
            // Create an array of object-Data pairs
            var aliveObjects = ManagerJobToData.Select((KeyValuePair<Verse.WeakReference<object>, Data> kv) => new KeyValuePair<object, Data>(kv.Key.Target, kv.Value))
                                               .Where((KeyValuePair<object, Data> kv) => kv.Key != null).ToList();
            var matches = aliveObjects.Where((KeyValuePair<object, Data> kv) => kv.Key == managerJob);

            Data toRet;
            if (matches.Count() == 0)
            {
                aliveObjects.Add(new KeyValuePair<object, Data>(managerJob, new Data()));
                toRet = aliveObjects.Last().Value;
            }
            else
            {
                toRet = matches.First().Value;
            }

            ManagerJobToData = aliveObjects.Select((KeyValuePair<object, Data> kv) => new KeyValuePair<Verse.WeakReference<object>, Data>(new Verse.WeakReference<object>(kv.Key), kv.Value)).ToList();

            return toRet;
        }

        public static bool ModRunning
        {
            get { return Verse.LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "fluffy.colonymanager" || x.Name == "Colony Manager"); }
        }

        public class Data : IExposable
        {
            public bool UseWithTaming = false;
            public bool UseWithButchering = false;
            public Dictionary<RimWorld.StatDef, float> Values = new Dictionary<RimWorld.StatDef, float>();
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

        static Texture2D _Cog;
        static Texture2D Cog
        {
            get
            {
                if (_Cog == null)
                    _Cog = (Texture2D)AccessTools.TypeByName("FluffyManager.Resources").GetField("Cog").GetValue(null);
                return _Cog;
            }
        }

        static FieldInfo _selectedCurrent;

        class SettingsWindow : Window
        {
            Data _Data;
            public SettingsWindow(Data data)
            {
                _Data = data;
                closeOnClickedOutside = true;
                draggable = true;
            }

            float DoSlider(Listing_Standard listingStandard, RimWorld.StatDef gene)
            {
                float startHeight = listingStandard.CurHeight;

                float newValue = listingStandard.Slider(_Data.Values[gene], 0.0001f, 0.999f);

                if (newValue == _Data.Values[gene])
                    return listingStandard.CurHeight - startHeight;

                _Data.Values[gene] = newValue;

                float others = _Data.Values.Where((KeyValuePair<RimWorld.StatDef, float> kv) => kv.Key != gene).Sum((KeyValuePair<RimWorld.StatDef, float> kv) => kv.Value);
                float modifier = (1.0f - newValue) / others;

                foreach (var key in _Data.Values.Keys.ToList())
                {
                    if (key == gene)
                        continue;
                    _Data.Values[key] = Math.Min(Math.Max(modifier * _Data.Values[key], 0.01f), 0.99f);
                }

                return listingStandard.CurHeight - startHeight;
            }

            public override Vector2 InitialSize => new Vector2(300f, 600);
            public override void DoWindowContents(Rect rect)
            {
                Widgets.DrawMenuSection(rect);

                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(rect.ContractedBy(Utilities.Margin));

                Text.Font = GameFont.Small;

                listingStandard.Label("AnimalGenetics.ColonyManager.Description".Translate());

                Vector2 position = new Vector2(rect.x, rect.y + listingStandard.CurHeight);
                float width = rect.width;

                Section(ref position, width - 2 * Utilities.Margin, DoCheckboxes, "AnimalGenetics.ColonyManager.UseWith".Translate());
                Section(ref position, width - 2 * Utilities.Margin, DoGenePreferences, "AnimalGenetics.ColonyManager.GeneImportance".Translate());

                listingStandard.End();
            }

            public float DoCheckboxes(Vector2 pos, float width)
            {
                //TooltipHandler.TipRegion(new Rect(pos.x, pos.y, width, 30), tooltip.Value);
                Utilities.DrawToggle(new Rect(pos.x, pos.y, width, 30), "FM.Livestock.TamingHeader".Translate(), "AnimalGenetics.ColonyManager.TamingTooltip".Translate(), ref _Data.UseWithTaming);
                Utilities.DrawToggle(new Rect(pos.x, pos.y+30, width, 30), "FM.Livestock.ButcherHeader".Translate(), "AnimalGenetics.ColonyManager.ButcheringTooltip".Translate(), ref _Data.UseWithButchering);
                return 60;
            }

            public float DoGenePreferences(Vector2 position, float width)
            {
                Rect rect = new Rect(position, new Vector2(width, 1000));

                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(rect);

                Text.Font = GameFont.Tiny;

                foreach (var gene in Constants.affectedStats)
                {
                    var label = Constants.GetLabel(gene);
                    var description = Constants.GetDescription(gene);
;
                    var highlightRect = listingStandard.Label(label, -1f, description);

                    highlightRect.height += DoSlider(listingStandard, gene);

                    Widgets.DrawHighlightIfMouseover(highlightRect);
                }

                listingStandard.End();

                return listingStandard.CurHeight;
            }
        };
        
        [HarmonyPatch]
        class My_Patch_Class
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                var template = typeof(Enumerable).GetMethods().FirstOrDefault((MethodInfo mi) => mi.Name == "OrderBy" && mi.GetParameters().Count() == 2);
                if (template == null)
                    return new List<MethodBase>();
                return new List<MethodBase> { template.MakeGenericMethod(typeof(Pawn), typeof(long)), template.MakeGenericMethod(typeof(Pawn), typeof(float)) };
            }

            static void Postfix(IEnumerable<Pawn> source, ref IOrderedEnumerable<Pawn> __result)
            {
                if (_CurrentJob != null)
                {
                    if (_IsButcherJob)
                        __result = __result.OrderBy((Pawn pawn) => CalculatePreferenceScore(GetDataOrCreate(_CurrentJob), pawn));
                    if (_IsTamingJob)
                        __result = __result.OrderByDescending((Pawn pawn) => CalculatePreferenceScore(GetDataOrCreate(_CurrentJob), pawn));
                }
            }
        }

        static float GetGene(Pawn pawn, RimWorld.StatDef gene)
        {
            if (gene == AnimalGenetics.GatherYield && !Genes.Gatherable(pawn))
                return 0.0f;
            return Genes.GetGene(pawn, gene);
        }

        public static float CalculatePreferenceScore(Data data, Pawn pawn)
        {
            return Constants.affectedStats.Select((RimWorld.StatDef gene) => GetGene(pawn, gene) * data.Values[gene]).Sum();
        }

        public static MethodInfo _ManagerTab_Livestock_DrawTamingSection;
        public static MethodInfo _Widgets_Section_Section;

        public static void Section(ref Vector2 position, float width, Func<Vector2, float, float> drawerFunc, string header = null, int id = 0)
        {
            var parameters = new object[] { position, width, drawerFunc, header, id };
            _Widgets_Section_Section.Invoke(null, parameters);
            position = (Vector2)parameters[0];
        }

        public static void Patch(Harmony h)
        {
            WasPatched = true;

            _selectedCurrent = AccessTools.TypeByName("FluffyManager.ManagerTab_Livestock").GetField("_selectedCurrent", BindingFlags.Instance | BindingFlags.NonPublic);

            _Widgets_Section_Section = AccessTools.Method(AccessTools.TypeByName("FluffyManager.Widgets_Section"), "Section");

            h.Patch(_Widgets_Section_Section,
                prefix: new HarmonyMethod(typeof(ColonyManager), nameof(ColonyManager.WidgetsSectionPrefix)));

            _ManagerTab_Livestock_DrawTamingSection = AccessTools.TypeByName("FluffyManager.ManagerTab_Livestock").GetMethod("DrawTamingSection", BindingFlags.Instance | BindingFlags.NonPublic);

            h.Patch(AccessTools.Method(AccessTools.TypeByName("FluffyManager.ManagerTab_Livestock"), "DoContent"),
                prefix: new HarmonyMethod(typeof(ColonyManager), nameof(ColonyManager.DoContent_Prefix)),
                postfix: new HarmonyMethod(typeof(ColonyManager), nameof(ColonyManager.DoContent_Postfix)));

            h.Patch(AccessTools.Method(AccessTools.TypeByName("FluffyManager.ManagerJob_Livestock"), "DoButcherJobs"),
                prefix: new HarmonyMethod(typeof(ColonyManager), nameof(ColonyManager.DoButcherJobs_Prefix)),
                postfix: new HarmonyMethod(typeof(ColonyManager), nameof(ColonyManager.DoButcherJobs_Postfix)));

            h.Patch(AccessTools.Method(AccessTools.TypeByName("FluffyManager.ManagerJob_Livestock"), "DoTamingJobs"),
                prefix: new HarmonyMethod(typeof(ColonyManager), nameof(ColonyManager.DoTamingJobs_Prefix)),
                postfix: new HarmonyMethod(typeof(ColonyManager), nameof(ColonyManager.DoTamingJobs_Postfix)));

            h.Patch(AccessTools.Method(AccessTools.TypeByName("FluffyManager.ManagerJob_Livestock"), "ExposeData"),
                postfix: new HarmonyMethod(typeof(ColonyManager), nameof(ColonyManager.ExposeDataPatch)));

            Utilities.Patch(h);
        }

        public static void ExposeDataPatch(object __instance)
        {
            if (Scribe.EnterNode("animalGenetics"))
            {
                GetDataOrCreate(__instance).ExposeData();
                Scribe.ExitNode();
            }
        }

        public static bool _IsButcherJob = false;
        public static bool _IsTamingJob = false;
        public static object _CurrentJob = null;
        public static bool DoContent_Prefix(object ____selectedCurrent)
        {
            _CurrentJob = ____selectedCurrent;
            return true;
        }

        public static void DoContent_Postfix()
        {
            _CurrentJob = null;
        }

        public static bool DoButcherJobs_Prefix(object __instance)
        {
            _CurrentJob = __instance;
            _IsButcherJob = true;
            return true;
        }

        public static void DoButcherJobs_Postfix()
        {
            _CurrentJob = null;
            _IsButcherJob = false;
        }

        public static bool DoTamingJobs_Prefix(object __instance)
        {
            _CurrentJob = __instance;
            _IsTamingJob = true;
            return true;
        }

        public static void DoTamingJobs_Postfix()
        {
            _CurrentJob = null;
            _IsTamingJob = false;
        }

        // This is so we can insert a new section above the taming section
        public static bool WidgetsSectionPrefix(ref Vector2 position, float width, Func<Vector2, float, float> drawerFunc, MethodInfo __originalMethod)
        {
            if (drawerFunc.GetMethodInfo() == _ManagerTab_Livestock_DrawTamingSection)
            {
                var arguments = new object[] { position, width, (Func<Vector2, float, float>)DrawAnimalGeneticSection, "AnimalGenetics", 0 };
                __originalMethod.Invoke(null, arguments);
                position = (Vector2)arguments[0];
            }
            return true;
        }

        private static float DrawAnimalGeneticSection(Vector2 pos, float width)
        {
            return Utilities.DrawCogButton(pos, width, "Configure Preferences", () => { Find.WindowStack.Add(new SettingsWindow(GetDataOrCreate(_CurrentJob))); }); ;
        }
    }
}
