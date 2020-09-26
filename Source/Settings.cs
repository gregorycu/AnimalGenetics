using Verse;
using UnityEngine;
using ExtensionMethods;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static void CheckboxLabeled(this Listing_Standard listingStandard, string label, ref bool checkOn, string tooltip, bool disabled)
        {
            float lineHeight = Text.LineHeight;
            Rect rect = listingStandard.GetRect(lineHeight);
            if (!tooltip.NullOrEmpty())
            {
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                TooltipHandler.TipRegion(rect, tooltip);
            }
            Widgets.CheckboxLabeled(rect, label, ref checkOn, disabled, null, null, false);
            listingStandard.Gap(listingStandard.verticalSpacing);
        }
    }
}

namespace AnimalGenetics
{
    public class AnimalGeneticsSettings : ModSettings
    {
        // Settings Menu
        public float stdDev = 0.12f;
        public float mean = 1f;
        public float mutationStdDev = 0.05f;
        public float mutationMean = 0.03f;
        public int colorMode = 1;
        public bool colorModeNormal = false;
        public bool colorModeRPG = true;
        public bool humanMode = false;
        public bool omniscientMode = true;
        // Other
        public int sortMode = 0;

        public bool ShowGenesInAnimalsTab
        {
            get { return current.showGenesInAnimalsTab; }
        }

        public bool ShowGenesInWildlifeTab
        {
            get { return current.showGenesInWildlifeTab; }
        }

        public bool ShowBothParentsInPawnTab
        {
            get { return current.showBothParentsInPawnTab; }
        }

        private struct Settings
        {
            public bool showGenesInAnimalsTab;
            public bool showGenesInWildlifeTab;
            public bool showBothParentsInPawnTab;
        };

        static Settings DefaultValues = new Settings
        {
            showGenesInAnimalsTab = false,
            showGenesInWildlifeTab = false,
            showBothParentsInPawnTab = false
        };

        Settings current = new Settings();
        Settings previous = new Settings();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref stdDev, "stdDev", 0.12f);
            Scribe_Values.Look<float>(ref mean, "mean", 1f);
            Scribe_Values.Look<float>(ref mutationStdDev, "mutationStdDev", 0.05f);
            Scribe_Values.Look<float>(ref mutationMean, "mutationMean", 0.03f);
            Scribe_Values.Look<int>(ref colorMode, "colorMode", 1);
            Scribe_Values.Look<bool>(ref humanMode, "humanMode", false);
            Scribe_Values.Look<bool>(ref omniscientMode, "omniscientMode", true);
            Scribe_Values.Look<bool>(ref current.showGenesInAnimalsTab, "showGenesInAnimalsTab", DefaultValues.showGenesInAnimalsTab);
            Scribe_Values.Look<bool>(ref current.showGenesInWildlifeTab, "showGenesInWildlifeTab", DefaultValues.showGenesInWildlifeTab);
            Scribe_Values.Look<bool>(ref current.showBothParentsInPawnTab, "showBothParentsInPawnTab", DefaultValues.showBothParentsInPawnTab);
        }

        public void DrawGraph(Rect rect, int xMin, int xMax, float mean, float stddev)
        {
            if (stddev < 0.5f)
                stddev = 0.5f;

            float height = rect.height;
            float width = rect.width;

            float vscale = (height - 30) / Value(mean);
            float hscale = (width - 40 - 20) / (xMax - xMin);

            GUI.BeginGroup(rect);

            //GUI.color = new Color(0.8f, 0.5f, 0.5f, 1f);
            //GUI.DrawTexture(new Rect(40, 10, width - 60, height - 20 - 10), TexUI.HighlightTex);
            GUI.color = Color.grey;

            Widgets.DrawLineHorizontal(40, height - 20, width - 60);

            if (xMin <= 0 && xMax >= 0)
                Widgets.DrawLineVertical(40 + -xMin * hscale, 10, height - 30);

            if (xMin <= 100 && xMax >= 100)
                Widgets.DrawLineVertical(40 + (100-xMin) * hscale, 10, height - 30);

            Text.Anchor = TextAnchor.MiddleCenter;

            Widgets.Label(new Rect(0, height - 30, 40, 20), "0%");
            Widgets.Label(new Rect(20, height - 20, 40, 20), xMin.ToString());
            Widgets.Label(new Rect(width - 40, height - 20, 40, 20), xMax.ToString());

            float Value(float x)
            {
                return (float)
                    (1f/stddev * System.Math.Sqrt(2f * System.Math.PI)) * (float)System.Math.Pow(System.Math.E, - (x - mean) * (x - mean) / (2f * stddev * stddev));
            }

            Widgets.Label(new Rect(0, 0, 40, 20), ((int)(100*Value(mean))).ToString()+"%");

            GUI.color = Color.white;

            var prev = new Vector2(xMin, Value(xMin));

            void DrawTo(Vector2 next)
            {
                Widgets.DrawLine(
                    new Vector2(40 + hscale * (prev.x - xMin), -20 + height - vscale * prev.y),
                    new Vector2(40 + hscale * (next.x - xMin), -20 + height - vscale * next.y),
                    Color.white, 1);
                prev = next;
            }

            for (int offsetX = -15; offsetX <= 15; ++offsetX)
            {
                float x = mean + stddev * (offsetX / 5f);
                if (x <= xMin || x >= xMax)
                    continue;
                DrawTo(new Vector2(x, Value(x)));
            }

            DrawTo(new Vector2(xMax, Value(xMax)));

            GUI.EndGroup();

            Text.Anchor = TextAnchor.UpperLeft;
        }

        public void DoSettingsWindowContents(Rect rect)
        {
            if (!omniscientMode)
                current.showGenesInWildlifeTab = false;

            float curY = 80;

            Rect generationGraph = new Rect(rect.width / 2 + 10, 0, rect.width / 2 - 10, 0);
            Rect mutationGraph = new Rect(rect.width / 2 + 10, 0, rect.width / 2 - 10, 0);

            Rect lhs = new Rect(10, curY, rect.width / 2 - 10, 400f);
            Rect topRhsRect = new Rect(10, curY, rect.width / 2 - 10, 200f);

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(lhs);

            generationGraph.y = listingStandard.CurHeight;

            listingStandard.Label("AG.Settings1".Translate(), -1f, "AG.Settings1Tooltip".Translate());
            listingStandard.Label("AG.Mean".Translate() + " : " + (mean * 100).ToString("F0"));
            mean = listingStandard.Slider(mean, 0f, 2f);
            listingStandard.Label("AG.StandardDeviation".Translate() + " : " + (stdDev * 100).ToString("F0"));
            stdDev = listingStandard.Slider(stdDev, 0f, 0.5f);

            generationGraph.height = listingStandard.CurHeight - generationGraph.y;

            listingStandard.Gap(20f);

            mutationGraph.y = listingStandard.CurHeight;

            listingStandard.Label("AG.Settings2".Translate(), -1f, "AG.Settings2Tooltip".Translate());
            listingStandard.Label("AG.Mean".Translate() + " : " + (mutationMean * 100).ToString("F0"));
            mutationMean = listingStandard.Slider(mutationMean, -0.25f, 0.25f);
            listingStandard.Label("AG.StandardDeviation".Translate() + " : " + (mutationStdDev * 100).ToString("F0"));
            mutationStdDev = listingStandard.Slider(mutationStdDev, 0f, 0.5f);

            mutationGraph.height = listingStandard.CurHeight - mutationGraph.y;

            listingStandard.Gap(20f);
            listingStandard.End();

            generationGraph.y += curY;
            mutationGraph.y += curY;
            DrawGraph(generationGraph, 0, 200, mean * 100, stdDev * 100);
            DrawGraph(mutationGraph, -25, 25, mutationMean * 100, mutationStdDev * 100);

            curY += listingStandard.CurHeight;

            Rect rect2 = new Rect(0, curY, rect.width / 2 - 10, 250f);
            Listing_Standard listingStandard2 = new Listing_Standard();
            listingStandard2.Begin(rect2);
            listingStandard2.Label("AG.ColorMode".Translate());
            if (listingStandard2.RadioButton_NewTemp("AG.ColorNormal".Translate(), colorModeNormal, 8f, "AG.ColorNormalTooltip".Translate(), 0f)) { colorModeNormal = true; colorModeRPG = false; colorMode = 0; }
            if (listingStandard2.RadioButton_NewTemp("AG.ColorRPG".Translate(), colorModeRPG, 8f, "AG.ColorRPGTooltip".Translate(), 0f)) { colorModeRPG = true; colorModeNormal = false; colorMode = 1; }
            listingStandard2.Gap(30f);
            listingStandard2.CheckboxLabeled("AG.HumanlikeGenes".Translate(), ref humanMode, "AG.HumanlikeGenesTooltip".Translate());
            listingStandard2.CheckboxLabeled("AG.Omniscient".Translate(), ref omniscientMode, "AG.OmniscientTooltip".Translate());
            listingStandard2.Gap(30f);
            listingStandard2.End();

            Rect rhs = new Rect(rect.width / 2 + 10, curY, rect.width / 2 - 10, 250f);
            Listing_Standard listingStandardRhs = new Listing_Standard();
            listingStandardRhs.Begin(rhs);
            listingStandardRhs.CheckboxLabeled("AnimalGenetics.ShowGenesInAnimalsTab".Translate(), ref current.showGenesInAnimalsTab, "AnimalGenetics.ShowGenesInAnimalsTabTooltip".Translate());
            listingStandardRhs.CheckboxLabeled("AnimalGenetics.ShowGenesInWildlifeTab".Translate(), ref current.showGenesInWildlifeTab, "AnimalGenetics.ShowGenesInWildlifeTabTooltip".Translate(), !omniscientMode);
            listingStandardRhs.CheckboxLabeled("AnimalGenetics.ShowBothParentsInPawnTab".Translate(), ref current.showBothParentsInPawnTab, "AnimalGenetics.ShowBothParentsInPawnTabTooltip".Translate(), !omniscientMode);

            listingStandardRhs.End();

            Listing_Standard bottom = new Listing_Standard();
            Rect bottomRect = new Rect(0, rect2.y + listingStandard2.CurHeight + 20, rect.width, 100);
            bottom.Begin(bottomRect);
			if (bottom.ButtonText("AG.DefaultSettings".Translate()))
            {
                stdDev = 0.12f;
                mean = 1f;
                mutationStdDev = 0.05f;
                mutationMean = 0.03f;
                colorMode = 1;
                colorModeNormal = false;
                colorModeRPG = true;
                humanMode = false;
                omniscientMode = true;
                current = DefaultValues;
            }
            bottom.End();

            if (!current.Equals(previous))
            {
                Assembly.AnimalGeneticsAssemblyLoader.PatchUI();
                previous = current;
            }

        }
    }
}