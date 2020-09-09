using Verse;
using UnityEngine;

namespace AnimalGenetics
{
    public class AnimalGeneticsSettings : ModSettings
    {
        public float mutationFactor = 0.15f;
        public float stdDev = 0.15f;
        public float mean = 1f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref mutationFactor, "mutationFactor", 0.15f);
            Scribe_Values.Look<float>(ref stdDev, "stdDev", 0.15f);
            Scribe_Values.Look<float>(ref mean, "mean", 1f);
        }

        public void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.Label("Genetic range for new animals (normally distributed)");
            listingStandard.Label("Mean : " + (mean * 100).ToString("F0"));
            mean = listingStandard.Slider(mean, 0f, 2f);
            listingStandard.Label("Standard Deviation : " + (stdDev * 100).ToString("F0"));
            stdDev = listingStandard.Slider(stdDev, 0f, 0.5f);
            listingStandard.Label("");
            listingStandard.Label("Inherited gene mutation factor (standard deviation from parent) : " + (mutationFactor * 100).ToString("F0"));
            mutationFactor = listingStandard.Slider(mutationFactor, 0f, 1f);
            listingStandard.End();
        }
    }
}