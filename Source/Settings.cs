using Verse;
using UnityEngine;

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
        }

        public void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.Label("AG.Settings1".Translate(), -1f, "AG.Settings1Tooltip".Translate());
            listingStandard.Label("AG.Mean".Translate() + " : " + (mean * 100).ToString("F0"));
            mean = listingStandard.Slider(mean, 0f, 2f);
            listingStandard.Label("AG.StandardDeviation".Translate() + " : " + (stdDev * 100).ToString("F0"));
            stdDev = listingStandard.Slider(stdDev, 0f, 0.5f);
            listingStandard.Gap(30f);
            listingStandard.Label("AG.Settings2".Translate(), -1f, "AG.Settings2Tooltip".Translate());
            listingStandard.Label("AG.Mean".Translate() + " : " + (mutationMean * 100).ToString("F0"));
            mutationMean = listingStandard.Slider(mutationMean, -0.25f, 0.25f);
            listingStandard.Label("AG.StandardDeviation".Translate() + " : " + (mutationStdDev * 100).ToString("F0"));
            mutationStdDev = listingStandard.Slider(mutationStdDev, 0f, 0.5f);
            //listingStandard.Gap(30f);
            listingStandard.End();

            float curY = listingStandard.CurHeight + 70f;
            Rect rect2 = new Rect(0, curY, rect.width / 2, 250f);
            Listing_Standard listingStandard2 = new Listing_Standard();
            listingStandard2.Begin(rect2);
            listingStandard2.Label("AG.ColorMode".Translate());
            if (listingStandard2.RadioButton_NewTemp("AG.ColorNormal".Translate(), colorModeNormal, 8f, "AG.ColorNormalTooltip".Translate(), 0f)) { colorModeNormal = true; colorModeRPG = false; colorMode = 0; }
            if (listingStandard2.RadioButton_NewTemp("AG.ColorRPG".Translate(), colorModeRPG, 8f, "AG.ColorRPGTooltip".Translate(), 0f)) { colorModeRPG = true; colorModeNormal = false; colorMode = 1; }
            listingStandard2.Gap(30f);
            listingStandard2.CheckboxLabeled("AG.HumanlikeGenes".Translate(), ref humanMode, "AG.HumanlikeGenesTooltip".Translate());
            listingStandard2.CheckboxLabeled("AG.Omniscient".Translate(), ref omniscientMode, "AG.OmniscientTooltip".Translate());
            listingStandard2.Gap(30f);
            if (listingStandard2.ButtonText("AG.DefaultSettings".Translate()))
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
    }
            listingStandard2.End();
        }
    }
}