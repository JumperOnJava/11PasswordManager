using Windows.UI;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;

namespace Password11.ColorLib;

public struct ColorsScheme
{
    public AdvColor BaseColor { get; set; }

    public AdvColor HoverColor { get; set; }

    public AdvColor SymbolColor { get; set; }

    public ColorsScheme(AdvColor baseColor, AdvColor hoverColor, AdvColor symbolColor)
    {
        BaseColor = baseColor;
        HoverColor = hoverColor;
        SymbolColor = symbolColor;
    }

    public ColorsScheme(AdvColor baseColor)
    {
        BaseColor = baseColor;
        var hoverColor = baseColor;

        hoverColor.S = hoverColor.S;
        hoverColor.S *= 0.66f;
        hoverColor.V *= 1.07f;

        HoverColor = hoverColor;

        SymbolColor = baseColor.S < 0.5f || (hoverColor.H > 40 && hoverColor.H < 200)
            ? new AdvColor(0, 0, 0)
            : new AdvColor(1, 1, 1);
    }

    [JsonIgnore]
    public static ColorsScheme AccentColors
    {
        get
        {
            var baseColor = new AdvColor((Color)Application.Current.Resources["SystemAccentColorLight2"]);
            var hoverColor = new AdvColor((Color)Application.Current.Resources["SystemAccentColorLight3"]);
            return new ColorsScheme(baseColor, hoverColor, new AdvColor(0, 0, 0));
        }
    }

    public override string ToString()
    {
        return $"{BaseColor.ToString()}|{HoverColor.ToString()}|{SymbolColor.ToString()}";
    }

    public static ColorsScheme FromString(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return AccentColors;

        var parts = s.Split('|');
        if (parts.Length != 3) return AccentColors;

        return new ColorsScheme
        {
            BaseColor = AdvColor.FromString(parts[0]),
            HoverColor = AdvColor.FromString(parts[1]),
            SymbolColor = AdvColor.FromString(parts[2])
        };
    }
}