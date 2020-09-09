using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalGenetics
{
    class MultiLerp
    {
        public MultiLerp(KeyValuePair<float, Color>[] points)
        {
            _Points = points;
        }
        KeyValuePair<float, Color>[] _Points;

        public Color Apply(float value)
        {
            if (value < _Points.First().Key)
                return _Points.First().Value;

            if (value > _Points.Last().Key)
                return _Points.Last().Value;

            var lhs = _Points.LastOrDefault((KeyValuePair<float, Color> point) => value >= point.Key);
            var rhs = _Points.FirstOrDefault((KeyValuePair<float, Color> point) => value < point.Key);

            Log.Message(">>> " + ((value - lhs.Key) / (rhs.Key - lhs.Key)).ToString());

            return Color.Lerp(lhs.Value, rhs.Value, (value - lhs.Key) / (rhs.Key - lhs.Key));
        }
    };


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
            /*KeyValuePair<float, Color>[] points = {
                    new KeyValuePair<float, Color>(0.80f, Color.gray),
                    new KeyValuePair<float, Color>(1.00f, new Color(0.1f, 0.7f, 0.1f)),
                    new KeyValuePair<float, Color>(1.20f, new Color(0.3f, 0.3f, 1.0f)),
                    new KeyValuePair<float, Color>(1.40f, new Color(0.5f, 0.2f, 0.7f)),
                    new KeyValuePair<float, Color>(1.60f, new Color(1.0f, 0.7f, 0.2f)),
                    new KeyValuePair<float, Color>(1.80f, Color.yellow)
            };
            var ml = new MultiLerp(points);

            return ml.Apply(mod);*/

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
            return (float)y1 * stdDev + mean;
        }

        public static int SampleInt()
        {
            return RandGen.Next();
        }
    }
}
