using ExtensionMethods;
using Verse;
using UnityEngine;

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
    class SettingsUI
    {
        public static void DrawGraph(Rect rect, int xMin, int xMax, float mean, float stddev)
        {
            if (stddev < 1f / System.Math.Sqrt(2f * System.Math.PI))
                stddev = (float)(1f / System.Math.Sqrt(2f * System.Math.PI));

            float height = rect.height;
            float width = rect.width;

            float vscale = (height - 30) / Value(mean);
            float hscale = (width - 40 - 20) / (xMax - xMin);

            GUI.BeginGroup(rect);

            GUI.color = Color.grey;

            Widgets.DrawLineHorizontal(40, height - 20, width - 60);

            if (xMin <= 0 && xMax >= 0)
                Widgets.DrawLineVertical(40 + -xMin * hscale, 10, height - 30);

            if (xMin <= 100 && xMax >= 100)
                Widgets.DrawLineVertical(40 + (100 - xMin) * hscale, 10, height - 30);

            Text.Anchor = TextAnchor.MiddleCenter;

            Widgets.Label(new Rect(0, height - 30, 40, 20), "0%");
            Widgets.Label(new Rect(20, height - 20, 40, 20), xMin.ToString());
            Widgets.Label(new Rect(40 - 20 + (width - 40 - 20) / 2, height - 20, 40, 20), (xMin + (xMax - xMin) / 2).ToString());
            Widgets.Label(new Rect(width - 40, height - 20, 40, 20), xMax.ToString());

            float Value(float x)
            {
                return (float)
                    (1f / (stddev * System.Math.Sqrt(2f * System.Math.PI))) * (float)System.Math.Pow(System.Math.E, -(x - mean) * (x - mean) / (2f * stddev * stddev));
            }

            Widgets.Label(new Rect(0, 0, 40, 20), ((int)(100 * Value(mean))).ToString() + "%");

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

        public static void DoSettings(CoreSettings settings, Rect rect)
        {
            float curY = 80;

            Rect generationGraph = new Rect(rect.width / 2 + 10, 0, rect.width / 2 - 10, 0);
            Rect mutationGraph = new Rect(rect.width / 2 + 10, 0, rect.width / 2 - 10, 0);

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(new Rect(10, curY, rect.width / 2 - 10, 400f));

            generationGraph.y = listingStandard.CurHeight;

            listingStandard.Label("AG.Settings1".Translate(), -1f, "AG.Settings1Tooltip".Translate());
            listingStandard.Label("AG.Mean".Translate() + " : " + (settings.mean * 100).ToString("F0"));
            settings.mean = listingStandard.Slider(settings.mean, 0f, 2f);
            listingStandard.Label("AG.StandardDeviation".Translate() + " : " + (settings.stdDev * 100).ToString("F0"));
            settings.stdDev = listingStandard.Slider(settings.stdDev, 0f, 0.5f);

            generationGraph.height = listingStandard.CurHeight - generationGraph.y;

            listingStandard.Gap(20f);

            mutationGraph.y = listingStandard.CurHeight;

            listingStandard.Label("AG.Settings2".Translate(), -1f, "AG.Settings2Tooltip".Translate());
            listingStandard.Label("AG.Mean".Translate() + " : " + (settings.mutationMean * 100).ToString("F0"));
            settings.mutationMean = listingStandard.Slider(settings.mutationMean, -0.25f, 0.25f);
            listingStandard.Label("AG.StandardDeviation".Translate() + " : " + (settings.mutationStdDev * 100).ToString("F0"));
            settings.mutationStdDev = listingStandard.Slider(settings.mutationStdDev, 0f, 0.5f);

            mutationGraph.height = listingStandard.CurHeight - mutationGraph.y;

            listingStandard.Gap(20f);

            generationGraph.y += curY;
            mutationGraph.y += curY;

            listingStandard.Label("AnimalGenetics.ChanceInheritBestGene".Translate() + " : " + (settings.bestGeneChance * 100).ToString("F0"));
            settings.bestGeneChance = listingStandard.Slider(settings.bestGeneChance, 0.0f, 1.0f);

            listingStandard.End();

            DrawGraph(generationGraph, 0, 200, settings.mean * 100, settings.stdDev * 100);
            DrawGraph(mutationGraph, -25, 25, settings.mutationMean * 100, settings.mutationStdDev * 100);

            curY += listingStandard.CurHeight;

            Listing_Standard listingStandard2 = new Listing_Standard();
            listingStandard2.Begin(new Rect(0, curY, rect.width / 2 - 10, 250f));

            listingStandard2.Gap(30f);
            listingStandard2.CheckboxLabeled("AG.HumanlikeGenes".Translate(), ref settings.humanMode, "AG.HumanlikeGenesTooltip".Translate());
            listingStandard2.CheckboxLabeled("AG.Omniscient".Translate(), ref settings.omniscientMode, "AG.OmniscientTooltip".Translate());

            if (listingStandard2.ButtonText("AG.DefaultSettings".Translate()))
                settings.Reset();

            listingStandard2.Gap(30f);
            listingStandard2.End();
        }

        public static void DoSettings(UISettings settings, Rect rect)
        {
            float curY = 80;

            Rect rect2 = new Rect(0, curY, rect.width / 2 - 10, 250f);
            Listing_Standard listingStandard2 = new Listing_Standard();
            listingStandard2.Begin(rect2);
            listingStandard2.Label("AG.ColorMode".Translate());
            if (listingStandard2.RadioButton("AG.ColorNormal".Translate(), settings.colorMode == 0, 8f, "AG.ColorNormalTooltip".Translate(), 0f)) { settings.colorMode = 0; }
            if (listingStandard2.RadioButton("AG.ColorRPG".Translate(), settings.colorMode == 1, 8f, "AG.ColorRPGTooltip".Translate(), 0f)) { settings.colorMode = 1; }
            listingStandard2.Gap(30f);
            listingStandard2.End();

            Rect rhs = new Rect(rect.width / 2 + 10, curY, rect.width / 2 - 10, 250f);
            Listing_Standard listingStandardRhs = new Listing_Standard();
            listingStandardRhs.Begin(rhs);
            listingStandardRhs.CheckboxLabeled("AnimalGenetics.ShowGenesInAnimalsTab".Translate(), ref settings.showGenesInAnimalsTab, "AnimalGenetics.ShowGenesInAnimalsTabTooltip".Translate());
            listingStandardRhs.CheckboxLabeled("AnimalGenetics.ShowGenesInWildlifeTab".Translate(), ref settings.showGenesInWildlifeTab, "AnimalGenetics.ShowGenesInWildlifeTabTooltip".Translate());
            listingStandardRhs.CheckboxLabeled("AnimalGenetics.ShowBothParentsInPawnTab".Translate(), ref settings.showBothParentsInPawnTab, "AnimalGenetics.ShowBothParentsInPawnTabTooltip".Translate());
            listingStandardRhs.CheckboxLabeled("AnimalGenetics.ShowGeneticsTab".Translate(), ref settings.showGeneticsTab, "AnimalGenetics.ShowGeneticsTabTooltip".Translate());

            listingStandardRhs.End();

            Listing_Standard bottom = new Listing_Standard();
            Rect bottomRect = new Rect(0, rect2.y + listingStandard2.CurHeight, rect.width, 100);
            bottom.Begin(bottomRect);
            if (bottom.ButtonText("AG.DefaultSettings".Translate()))
            {
                settings.Reset();
            }
            bottom.End();

            Assembly.AnimalGeneticsAssemblyLoader.PatchUI();
        }

        public static void DoSettings(IntegrationSettings settings, Rect rect)
        {
            float curY = 80;

            Rect rect2 = new Rect(0, curY, rect.width / 2 - 10, 250f);
            Listing_Standard listingStandard2 = new Listing_Standard();
            listingStandard2.Begin(rect2);

            listingStandard2.Gap(30f);

            TaggedString warning = (settings.ColonyManagerIntegration == ColonyManager.WasPatched)
                ? new TaggedString("")
                : new TaggedString(" (") + "AnimalGenetics.NeedsRestart".Translate() + new TaggedString(")");

            listingStandard2.CheckboxLabeled("AnimalGenetics.ColonyManager.Integrate".Translate() + warning, ref settings.ColonyManagerIntegration, "AnimalGenetics.ColonyManager.IntegrateTooltip".Translate());

            listingStandard2.Gap(30f);
            listingStandard2.End();

            Listing_Standard bottom = new Listing_Standard();
            Rect bottomRect = new Rect(0, rect2.y + listingStandard2.CurHeight, rect.width, 100);
            bottom.Begin(bottomRect);
            if (bottom.ButtonText("AG.DefaultSettings".Translate()))
            {
                settings.Reset();
            }
            bottom.End();

            Assembly.AnimalGeneticsAssemblyLoader.PatchUI();
        }
    }
}
