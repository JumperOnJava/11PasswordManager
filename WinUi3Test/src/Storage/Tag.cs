using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Microsoft.UI.Xaml.Media;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.Storage
{
    public interface Tag : Identifiable
    {
        ColorsScheme TagColors { get; set; }
        string DisplayName { get; set; }
        bool matches(Taggable tag);
        public Brush BaseColorBrush { get; }
        public Brush HoverColorBrush { get; }
        public Brush SymbolColorBrush { get; }
    }
    public struct TagBasic : Tag
    {
        public string DisplayName { get; set; }
        public long identifier;
        public long Identifier { get => identifier; private set => identifier=value; }
        public ColorsScheme TagColors { get; set; }

        public Brush BaseColorBrush => TagColors.BaseColor.asBrush;
        public Brush HoverColorBrush => TagColors.HoverColor.asBrush;
        public Brush SymbolColorBrush => TagColors.SymbolColor.asBrush;
        
        public bool matches(Taggable account)
        {
            var whereBasic = account.Tags.Where(a => a is Identifiable).ToList();
            var reference = new{this.identifier};
            var whereIdentifier = whereBasic.Where((a) => a.Identifier == reference.identifier).ToList();
            var count = whereIdentifier.Count();
            if (count > 0)
                return true;
            return false;
        }
        public TagBasic(string displayName, ColorsScheme TagColor)
        {
            DisplayName = displayName;
            identifier = Random.Shared.NextInt64();
            TagColors = TagColor;
        }
        public TagBasic(string displayName) : this(displayName, ColorsScheme.AccentColors) { } 
        public static TagBasic createRandom()
        {
            return new TagBasic("tag" + Random.Shared.Next(100), ColorsScheme.AccentColors);
        }
    }
}