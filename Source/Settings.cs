using System;
using Verse;

namespace AnimalGenetics
{
    public class CoreSettings : ModSettings, ICloneable
    {
        public float stdDev;
        public float mean;
        public float mutationStdDev;
        public float mutationMean;
        public bool humanMode;
        public bool omniscientMode;
        public float bestGeneChance;

        public CoreSettings()
        {
            Reset();
        }

        public void Reset()
        {
            stdDev = 0.12f;
            mean = 1f;
            mutationStdDev = 0.05f;
            mutationMean = 0.03f;
            humanMode = false;
            omniscientMode = true;
            bestGeneChance = 0.5f;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override void ExposeData()
        {
            var defaults = new CoreSettings();
            Scribe_Values.Look(ref stdDev, "stdDev", defaults.stdDev);
            Scribe_Values.Look(ref mean, "mean", defaults.mean);
            Scribe_Values.Look(ref mutationStdDev, "mutationStdDev", defaults.mutationStdDev);
            Scribe_Values.Look(ref mutationMean, "mutationMean", defaults.mutationMean);
            Scribe_Values.Look(ref humanMode, "humanMode", defaults.humanMode);
            Scribe_Values.Look(ref omniscientMode, "omniscientMode", defaults.omniscientMode);
            Scribe_Values.Look(ref bestGeneChance, "bestGeneChance", defaults.bestGeneChance);
        }
    }

    public class UISettings : ModSettings, ICloneable
    {
        public int colorMode;
        public bool showGenesInAnimalsTab;
        public bool showGenesInWildlifeTab;
        public bool showBothParentsInPawnTab;
        public int sortMode;
        public UISettings()
        {
            Reset();
        }

        public void Reset()
        {
            colorMode = 1;
            showGenesInAnimalsTab = false;
            showGenesInWildlifeTab = false;
            showBothParentsInPawnTab = false;
			sortMode = 0;
		}

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override void ExposeData()
        {
            var defaults = new UISettings();
            Scribe_Values.Look(ref colorMode, "colorMode", defaults.colorMode);
            Scribe_Values.Look(ref showGenesInAnimalsTab, "showGenesInAnimalsTab", defaults.showGenesInAnimalsTab);
            Scribe_Values.Look(ref showGenesInWildlifeTab, "showGenesInWildlifeTab", defaults.showGenesInWildlifeTab);
            Scribe_Values.Look(ref showBothParentsInPawnTab, "showBothParentsInPawnTab", defaults.showBothParentsInPawnTab);
        }
    }
}