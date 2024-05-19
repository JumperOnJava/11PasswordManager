using Microsoft.UI.Xaml.Media;
using System;
using System.Text.Json.Serialization;
using Windows.UI;
using Newtonsoft.Json;

namespace WinUi3Test.src.Ui
{
    public struct AdvColor
    {
        [JsonRequired]
        public int G { get; set; }
        [JsonRequired]
        public int R { get; set; }
        
        [JsonRequired]
        public int B { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Color asWinColor => new() { A= 255,R=(byte)R,G=(byte)G,B=(byte)B};

        [Newtonsoft.Json.JsonIgnore]
        public SolidColorBrush asBrush => new(asWinColor);

        public AdvColor(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
        public AdvColor(float r, float g, float b)
        {
            R = (int)(r * 255);
            G = (int)(g * 255);
            B = (int)(b * 255);
        }

        public AdvColor(Color color) : this(color.R, color.G, color.B)
        {

        }

        
        [Newtonsoft.Json.JsonIgnore]
        public float r
        {
            get
            {
                return R / 255f;
            }
            set
            {
                R = (int)(value * 255);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public float g
        {
            get
            {
                return G / 255f;
            }
            set
            {
                G = (int)(value * 255);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public float b
        {
            get
            {
                return B / 255f;
            }
            set
            {
                B = (int)(value * 255);
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public float H
        {
            get
            {
                float hue, saturation, value;
                RgbToHsv(R, G, B, out hue, out saturation, out value);
                return (float)hue;
            }
            set
            {
                int r, g, b;
                HsvToRgb(value, S, V, out r, out g, out b);
                R = r;
                G = g;
                B = b;
            }
        }

        // Property for Saturation component
        [Newtonsoft.Json.JsonIgnore]
        public float S
        {
            get
            {
                float hue, saturation, value;
                RgbToHsv(R, G, B, out hue, out saturation, out value);
                return (float)saturation;
            }
            set
            {
                int r, g, b;
                if (value > 1)
                    value = 1;
                HsvToRgb(H, value, V, out r, out g, out b);
                R = r;
                G = g;
                B = b;
            }
        }

        // Property for Value component
        [Newtonsoft.Json.JsonIgnore]
        public float V
        {
            get
            {
                float hue, saturation, value;
                RgbToHsv(R, G, B, out hue, out saturation, out value);
                return (float)value;
            }
            set
            {
                int r, g, b;

                if (value > 1)
                    value = 1;
                HsvToRgb(H, S, value, out r, out g, out b);
                R = r;
                G = g;
                B = b;
            }
        }

        public static void HsvToRgb(float hue, float saturation, float value, out int r, out int g, out int b)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = (byte)(value);
            byte p = (byte)(value * (1 - saturation));
            byte q = (byte)(value * (1 - f * saturation));
            byte t = (byte)(value * (1 - (1 - f) * saturation));
            if (hi == 0)
            {
                r = v;
                g = t;
                b = p;
            }
            else if (hi == 1)
            {
                r = q;
                g = v;
                b = p;
            }
            else if (hi == 2)
            {
                r = p;
                g = v;
                b = t;
            }
            else if (hi == 3)
            {
                r = p;
                g = q;
                b = v;
            }
            else if (hi == 4)
            {
                r = t;
                g = p;
                b = v;
            }
            else
            {
                r = v;
                g = p;
                b = q;
            }

        }
        public static void RgbToHsv(int ri, int gi, int bi, out float hue, out float saturation, out float value)
        {
            float r = ri / 255f;
            float g = gi / 255f;
            float b = bi / 255f;
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;
            float[] hsv = new float[3];

            hsv[1] = max != 0 ? delta / max : 0;
            hsv[2] = max;

            if (hsv[1] == 0)
            {
                hue = hsv[0];
                saturation = hsv[1];
                value = hsv[2];
                return;
            }

            if (r == max)
            {
                hsv[0] = ((g - b) / delta);
            }
            else if (g == max)
            {
                hsv[0] = ((b - r) / delta) + 2.0f;
            }
            else if (b == max)
            {
                hsv[0] = ((r - g) / delta) + 4.0f;
            }

            hsv[0] *= 60.0f;

            if (hsv[0] < 0)
            {
                hsv[0] += 360.0f;
            }

            hue = hsv[0];
            saturation = hsv[1];
            value = hsv[2];
        }

        public override string ToString()
        {
            return $"{R:X}{G:X}{B:X}";
        }
    }

}
