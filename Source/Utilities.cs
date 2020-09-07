using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public static class Utilities
    {
        private static float LowEnd = 0.5f;
        private static float Mid = 1f;
        private static float HighEnd = 1.5f;
        private static float LowerMult = 1 / (Mid - LowEnd);
        private static float UpperMult = 1 / (HighEnd - Mid);
        private static Color LowEndColor = new Color(0.4f, 0f, 0f);
        private static Color MidColor = new Color(1f, 1f, 0.4f);
        private static Color HighEndColor = new Color(0.4f, 1f, 0.4f);

        public static Color TextColor(float mod)
        {
            if (mod > Mid) {
                return Color.Lerp(MidColor, HighEndColor, (mod - Mid) * UpperMult);
            }
            else {
                return Color.Lerp(MidColor, LowEndColor, (Mid - mod) * LowerMult);
            }
        }
    }
}
