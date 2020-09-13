using Verse;
using UnityEngine;

namespace AnimalGenetics
{
    public class AnimalGeneticsSettings : ModSettings
    {
        public float stdDev = 0.12f;
        public float mean = 1f;
        public float mutationStdDev = 0.05f;
        public float mutationMean = 0.03f;
        public int colorMode = 1;
        public bool colorModeNormal = false;
        public bool colorModeRPG = true;
        public bool humanMode = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref stdDev, "stdDev", 0.12f);
            Scribe_Values.Look<float>(ref mean, "mean", 1f);
            Scribe_Values.Look<float>(ref mutationStdDev, "mutationStdDev", 0.05f);
            Scribe_Values.Look<float>(ref mutationMean, "mutationMean", 0.03f);
            Scribe_Values.Look<int>(ref colorMode, "colorMode", 1);
            Scribe_Values.Look<bool>(ref humanMode, "humanMode", false);
        }

        public void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.Label("Genetic range for new animals", -1f, "Randomly selected from a normal distribution with these settings...");
            listingStandard.Label("Mean : " + (mean * 100).ToString("F0"));
            mean = listingStandard.Slider(mean, 0f, 2f);
            listingStandard.Label("Standard Deviation : " + (stdDev * 100).ToString("F0"));
            stdDev = listingStandard.Slider(stdDev, 0f, 0.5f);
            listingStandard.Gap(30f);
            listingStandard.Label("Inherited gene mutation factor", -1f, "Added/subtracted from inherited genes. Randomly selected from a normal distribution with these settings...");
            listingStandard.Label("Mean : " + (mutationMean * 100).ToString("F0"));
            mutationMean = listingStandard.Slider(mutationMean, -0.25f, 0.25f);
            listingStandard.Label("Standard Deviation : " + (mutationStdDev * 100).ToString("F0"));
            mutationStdDev = listingStandard.Slider(mutationStdDev, 0f, 0.5f);
            //listingStandard.Gap(30f);
            listingStandard.End();

            float curY = listingStandard.CurHeight + 70f;
            Rect rect2 = new Rect(0, curY, rect.width / 2, 200f);
            Listing_Standard listingStandard2 = new Listing_Standard();
            listingStandard2.Begin(rect2);
            listingStandard2.Label("Color Mode");
            if (listingStandard2.RadioButton_NewTemp("Normal", colorModeNormal, 8f, "Traditional red->yellow->green colors", 0f)) { colorModeNormal = true; colorModeRPG = false; colorMode = 0; }
            if (listingStandard2.RadioButton_NewTemp("RPG", colorModeRPG, 8f, "RPG style item quality colors", 0f)) { colorModeRPG = true; colorModeNormal = false; colorMode = 1; }
            listingStandard2.Gap(30f);
            listingStandard2.CheckboxLabeled("Apply Genes to Humanlikes", ref humanMode, "Gives genes to all humanlike pawns as well as animals");
            listingStandard2.Gap(30f);
            if (listingStandard2.ButtonText("Default Settings"))
            {
                stdDev = 0.12f;
                mean = 1f;
                mutationStdDev = 0.05f;
                mutationMean = 0.03f;
                colorMode = 1;
                colorModeNormal = false;
                colorModeRPG = true;
                humanMode = false;
            }
            listingStandard2.End();
        }
    }
}