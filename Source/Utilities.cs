using RimWorld;
using UnityEngine;
using Verse;
using System;

namespace AnimalGenetics
{
    public static class Utilities
    {
        private static float LowEnd = 0.5f;
        private static float Mid = 1f;
        private static float HighEnd = 1.5f;
        private static float LowerMult = 1 / (Mid - LowEnd);
        private static float UpperMult = 1 / (HighEnd - Mid);
        private static Color LowEndColor = new Color(0.9f, 0f, 0f);
        private static Color MidColor = new Color(1f, 1f, 0.4f);
        private static Color HighEndColor = new Color(0.4f, 1f, 0.4f);
        private static System.Random RandGen = new System.Random();

        public static Color TextColor(float mod)
        {
            if (mod > Mid)
            {
                return Color.Lerp(MidColor, HighEndColor, (mod - Mid) * UpperMult);
            }
            else
            {
                return Color.Lerp(MidColor, LowEndColor, (Mid - mod) * LowerMult);
            }
        }

        // Cheers, https://gist.github.com/tansey/1444070
        public static float SampleGaussian(float mean, float stdDev, float lowerBound)
        {
            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            double x1 = 1 - RandGen.NextDouble();
            double x2 = 1 - RandGen.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            float ret = (float)y1 * stdDev + mean;
            if (ret < lowerBound)
            {
                return lowerBound;
            }
            return ret;
        }

        public static float SampleGaussian(float mean, float stdDev)
        {
            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            double x1 = 1 - RandGen.NextDouble();
            double x2 = 1 - RandGen.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return ret = (float)y1 * stdDev + mean;
        }

        public static int SampleInt()
        {
            return RandGen.Next();
        }
    }
}
