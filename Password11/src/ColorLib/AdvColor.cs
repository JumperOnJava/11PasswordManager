using System;
using Windows.UI;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Password11.ColorLib;

public struct AdvColor
{
    [JsonRequired] public int R { get; set; }
    [JsonRequired] public int G { get; set; }
    [JsonRequired] public int B { get; set; }

    [JsonIgnore] public Color AsWinColor => new() { A = 255, R = (byte)R, G = (byte)G, B = (byte)B };

    [JsonIgnore] public SolidColorBrush AsBrush => new(AsWinColor);

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


    [JsonIgnore]
    public float Rf
    {
        get => R / 255f;
        set => R = (int)(value * 255);
    }

    [JsonIgnore]
    public float Gf
    {
        get => G / 255f;
        set => G = (int)(value * 255);
    }

    [JsonIgnore]
    public float Bf
    {
        get => B / 255f;
        set => B = (int)(value * 255);
    }

    [JsonIgnore]
    public float H
    {
        get
        {
            RgbToHsv(R, G, B, out var hue, out _, out _);
            return hue;
        }
        set
        {
            HsvToRgb(value, S, V, out var r, out var g, out var b);
            R = r;
            G = g;
            B = b;
        }
    }

    // Property for Saturation component
    [JsonIgnore]
    public float S
    {
        get
        {
            RgbToHsv(R, G, B, out _, out var saturation, out _);
            return saturation;
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
    [JsonIgnore]
    public float V
    {
        get
        {
            float value;
            RgbToHsv(R, G, B, out _, out _, out value);
            return value;
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
        var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        var f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        var v = (byte)value;
        var p = (byte)(value * (1 - saturation));
        var q = (byte)(value * (1 - f * saturation));
        var t = (byte)(value * (1 - (1 - f) * saturation));
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
        var r = ri / 255f;
        var g = gi / 255f;
        var b = bi / 255f;
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;
        var hsv = new float[3];

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
            hsv[0] = (g - b) / delta;
        else if (g == max)
            hsv[0] = (b - r) / delta + 2.0f;
        else if (b == max) hsv[0] = (r - g) / delta + 4.0f;

        hsv[0] *= 60.0f;

        if (hsv[0] < 0) hsv[0] += 360.0f;

        hue = hsv[0];
        saturation = hsv[1];
        value = hsv[2];
    }

    public override string ToString()
    {
        return $"{R}-{G}-{B}";
    }

    public static AdvColor FromString(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Input string cannot be null or empty", nameof(s));

        var parts = s.Split('-');
        if (parts.Length != 3) throw new FormatException("Input string must be in the format 'R-G-B'");

        if (!int.TryParse(parts[0], out var r) ||
            !int.TryParse(parts[1], out var g) ||
            !int.TryParse(parts[2], out var b))
            throw new FormatException("Each component must be a valid integer");

        return new AdvColor { R = r, G = g, B = b };
    }
}