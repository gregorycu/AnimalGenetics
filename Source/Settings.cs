using Verse;
using UnityEngine;

namespace AnimalGenetics
{
    public class AnimalGeneticsSettings : ModSettings
    {
        public float mutationFactor = 0.1f;
        public float stdDev = 0.15f;
        public float mean = 1f;
        public int colorMode = 1;
        public bool colorModeNormal = false;
        public bool colorModeRPG = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref mutationFactor, "mutationFactor", 0.10f);
            Scribe_Values.Look<float>(ref stdDev, "stdDev", 0.10f);
            Scribe_Values.Look<float>(ref mean, "mean", 1f);
            Scribe_Values.Look<int>(ref colorMode, "colorMode", 1);
        }

        public void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.Label("Genetic range for new animals", -1f, "Randomly selected from a normal distribution with these settings");
            listingStandard.Label("Mean : " + (mean * 100).ToString("F0"));
            mean = listingStandard.Slider(mean, 0f, 2f);
            listingStandard.Label("Standard Deviation : " + (stdDev * 100).ToString("F0"));
            stdDev = listingStandard.Slider(stdDev, 0f, 0.5f);
            listingStandard.Gap(30f);
            listingStandard.Label("Inherited gene mutation factor : " + (mutationFactor * 100).ToString("F0"), -1f, "Random normally distributed difference from parent values, mean = 0, std. deviation = <mutation factor>");
            mutationFactor = listingStandard.Slider(mutationFactor, 0f, 0.5f);
            //listingStandard.Gap(30f);
            listingStandard.End();

            float curY = listingStandard.CurHeight + 70f;
            Rect rect2 = new Rect(0, curY, rect.width / 2, 150f);
            Listing_Standard listingStandard2 = new Listing_Standard();
            listingStandard2.Begin(rect2);
            listingStandard2.Label("Color Mode");
            if (listingStandard2.RadioButton_NewTemp("Normal", colorModeNormal, 8f, "Traditional red->yellow->green colors", 0f)) { colorModeNormal = true; colorModeRPG = false; colorMode = 0; }
            if (listingStandard2.RadioButton_NewTemp("RPG", colorModeRPG, 8f, "RGP style item quality colors", 0f)) { colorModeRPG = true; colorModeNormal = false; colorMode = 1; }
            listingStandard2.Gap(30f);
            if (listingStandard2.ButtonText("Default Settings"))
            {
                mutationFactor = 0.1f;
                stdDev = 0.15f;
                mean = 1f;
                colorMode = 1;
                colorModeNormal = false;
                colorModeRPG = true;
            }
            listingStandard2.End();
        }
    }
}
