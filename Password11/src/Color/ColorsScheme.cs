﻿using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Newtonsoft.Json;

namespace Password11.src.Ui
{
    public struct ColorsScheme
    {
        private AdvColor baseColor;
        public AdvColor BaseColor
        {
            get => baseColor;
            set => baseColor = value;
        }

        private AdvColor hoverColor;
        public AdvColor HoverColor
        {
            get => hoverColor;
            set => hoverColor = value;
        }

        private AdvColor symbolColor;
        public AdvColor SymbolColor
        {
            get => symbolColor;
            set => symbolColor = value;
        }

        public ColorsScheme(AdvColor baseColor, AdvColor hoverColor, AdvColor symbolColor)
        {
            this.baseColor = baseColor;
            this.hoverColor = hoverColor;
            this.symbolColor = symbolColor;
        }
        public ColorsScheme(AdvColor baseColor)
        {
            this.baseColor = baseColor;
            this.hoverColor = baseColor;

            Console.WriteLine($"S1 H{hoverColor.H} S:{hoverColor.S} V:{hoverColor.V}");
            hoverColor.S = hoverColor.S;
            Console.WriteLine($"S2 H{hoverColor.H} S:{hoverColor.S} V:{hoverColor.V}");
            hoverColor.S *= 0.66f;
            Console.WriteLine($"S3 H{hoverColor.H} S:{hoverColor.S} V:{hoverColor.V}");
            hoverColor.V *= 1.07f;
            symbolColor = (baseColor.S<0.5f || (hoverColor.H > 40 && hoverColor.H < 200)) ? new AdvColor(0, 0, 0) : new AdvColor(1,1,1);
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
            if (string.IsNullOrWhiteSpace(s))
            {
                return AccentColors;
            }

            var parts = s.Split('|');
            if (parts.Length != 3)
            {
                return AccentColors;
            }

            return new ColorsScheme
            {
                BaseColor = AdvColor.FromString(parts[0]),
                HoverColor = AdvColor.FromString(parts[1]),
                SymbolColor = AdvColor.FromString(parts[2])
            };
        }
    }

}
