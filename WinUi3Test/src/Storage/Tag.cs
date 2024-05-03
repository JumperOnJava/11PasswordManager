using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.Storage
{
    public interface Tag : Identifiable
    {
        static Tag Any = TagAny.Instance;
        ColorsScheme TagColors { get; set; }
        string DisplayName { get; set; }
        bool matches(Taggable tag);
    }
    public class TagAny : Tag
    {
        public static TagAny Instance = new TagAny();
        public long Identifier { get; }

        public string DisplayName { get { return displayName; } set { } }

        public ColorsScheme TagColors { get { return ColorsScheme.AccentColors; } set { } }

        private string displayName = "Any";

        public bool matches(Taggable tag) => true;
        private TagAny() { Identifier = 0; }
    }
    public class TagBasic : Tag
    {
        public string DisplayName { get; set; }
        public long Identifier { get; private set; }
        public ColorsScheme TagColors { get; set; }

        public bool matches(Taggable account)
        {
            if (account.Tags.Where(a => a is TagBasic).Where((a) => ((TagBasic)a).Identifier == Identifier).Count() > 0)
                return true;
            return false;
        }
        public TagBasic(string displayName, ColorsScheme TagColor)
        {
            DisplayName = displayName;
            Identifier = Random.Shared.NextInt64();
            this.TagColors = TagColor;
        }
        public TagBasic(string displayName) : this(displayName, ColorsScheme.AccentColors) { } 
        public static TagBasic createRandom()
        {
            return new TagBasic("tag" + Random.Shared.Next(100), ColorsScheme.AccentColors);
        }
    }
}