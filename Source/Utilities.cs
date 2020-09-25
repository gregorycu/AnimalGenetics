using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AnimalGenetics
{
    namespace Utility
    {
        class GUI
        {
            public static void DrawGeneValueLabel(Rect box, float value, bool strikethrough = false, string extra = "")
            {
                TextAnchor oldTextAnchor = Text.Anchor;
                Color oldColor = UnityEngine.GUI.color;

                Text.Anchor = TextAnchor.MiddleCenter;
                UnityEngine.GUI.color = Utilities.TextColor(value);

                string text = (value * 100).ToString("F0") + "%" + extra;
                Widgets.Label(box, text);

                if (strikethrough)
                {
                    float halfSize = Text.CalcSize(text).x / 2 + 1;
                    float midpoint = (box.xMin + box.xMax) / 2;

                    Widgets.DrawLine(new Vector2(midpoint- halfSize, box.y + box.height / 2 - 1), new Vector2(midpoint + halfSize, box.y + box.height / 2 - 1), new Color(1f, 1f, 1f, 0.5f), 1);
                }

                UnityEngine.GUI.color = oldColor;
                Text.Anchor = oldTextAnchor;
            }
        };
    }
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

            return Color.Lerp(lhs.Value, rhs.Value, (value - lhs.Key) / (rhs.Key - lhs.Key));
        }
    };


    public static class Utilities
    {
        private static System.Random RandGen = new System.Random();

        private static KeyValuePair<float, Color>[] pointsRPG = {
            new KeyValuePair<float, Color>(0.80f, Color.gray),
            new KeyValuePair<float, Color>(0.95f, Color.white),
            new KeyValuePair<float, Color>(1.05f, new Color(0.1f, 0.7f, 0.1f)),
            new KeyValuePair<float, Color>(1.20f, new Color(0.3f, 0.3f, 1.0f)),
            new KeyValuePair<float, Color>(1.40f, new Color(0.5f, 0.2f, 0.7f)),
            new KeyValuePair<float, Color>(1.60f, new Color(1.0f, 0.6f, 0.1f)),
            new KeyValuePair<float, Color>(1.80f, Color.yellow)
        };

        private static KeyValuePair<float, Color>[] pointsNormal = {
            new KeyValuePair<float, Color>(0.5f, new Color(0.9f, 0f, 0f)),
            new KeyValuePair<float, Color>(1.0f, Color.yellow),
            new KeyValuePair<float, Color>(1.70f, Color.green)
        };

        private static List<KeyValuePair<float, Color>[]> colorProfiles = new List<KeyValuePair<float, Color>[]>()
        {
            pointsNormal,
            pointsRPG
        };

        public static Color TextColor(float mod)
        {
            KeyValuePair<float, Color>[] points;
            points = colorProfiles[Controller.Settings.colorMode];
            var ml = new MultiLerp(points);

            return ml.Apply(mod);
        }

        // Cheers, https://gist.github.com/tansey/1444070
        public static float SampleGaussian(float mean, float stdDev, float lowerBound)
        {
            double x1 = 1 - RandGen.NextDouble();
            double x2 = 1 - RandGen.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            float ret = ((float)y1) * stdDev + mean;
            if (ret < lowerBound)
            {
                return lowerBound;
            }
            return ret;
        }

        public static float SampleGaussian(float mean, float stdDev)
        {
            double x1 = 1 - RandGen.NextDouble();
            double x2 = 1 - RandGen.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return ((float)y1) * stdDev + mean;
        }

        public static int SampleInt()
        {
            return RandGen.Next();
        }
    }
}
