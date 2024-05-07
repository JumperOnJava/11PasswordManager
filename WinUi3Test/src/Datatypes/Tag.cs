using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Windows.UI;
using Microsoft.UI.Xaml.Media;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.Storage
{
    public class TagRef : Tag
    {
        [JsonInclude]
        public long innerId;
        
        public TagRef(long innerId)
        {
            this.innerId = innerId;   
        }
        public override int GetHashCode()
        {
            return (int)innerId;
        }
        public static bool operator ==(TagRef a, TagRef b)
        {
            return a.innerId == b.innerId;
        }
        public static bool operator !=(TagRef a, TagRef b)
        {
            return !(a == b);
        }
        [JsonIgnore]
        public ColorsScheme TagColors
        {
            get => TagManager.Instance.getTag(this).TagColors; set
            {
                var tag = TagManager.Instance.getTag(this);
                tag.TagColors = value;
                TagManager.Instance.setTag(this, tag);
            }
        }
        [JsonIgnore]
        public string DisplayName
        {
            get => TagManager.Instance.getTag(this).DisplayName;
            set
            {
                var tag = TagManager.Instance.getTag(this);
                tag.DisplayName = value;
                TagManager.Instance.setTag(this, tag);
            }
        }

        [JsonIgnore]
        public Brush BaseColorBrush => TagManager.Instance.getTag(this).BaseColorBrush;

        [JsonIgnore]
        public Brush HoverColorBrush => TagManager.Instance.getTag(this).HoverColorBrush;

        [JsonIgnore]
        public Brush SymbolColorBrush => TagManager.Instance.getTag(this).SymbolColorBrush;

        [JsonIgnore]
        public TagRef Identifier => this;

        public Tag Clone()
        {
            return this;
        }

        public bool matches(Taggable tag)
        {
            return TagManager.Instance.getTag(this).matches(tag);
        }
    }

    public class TagManager
    {
        public static TagManager Instance = new TagManager();
        public Dictionary<long, Tag> tags = new Dictionary<long, Tag>();

        private TagManager()
        {
        }

        public Tag getTag(TagRef tagRef)
        {
            return tags[tagRef.innerId];
        }

        public void setTag(TagRef tagRef, Tag tag)
        {
            tags[tagRef.innerId] = tag;
        }
    }

    public interface Tag : Identifiable, Clonable<Tag>
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
        [JsonInclude]
        public TagRef identifier;
        public TagRef Identifier { get => identifier; private set => identifier = value; }
        public ColorsScheme TagColors { get; set; }

        public Brush BaseColorBrush => TagColors.BaseColor.asBrush;
        public Brush HoverColorBrush => TagColors.HoverColor.asBrush;
        public Brush SymbolColorBrush => TagColors.SymbolColor.asBrush;

        public bool matches(Taggable account)
        {
            var whereBasic = account.Tags.Where(a => a is Identifiable).ToList();
            var r = new { this.Identifier };
            var whereIdentifier = whereBasic.Where((a) => a == r.Identifier).ToList();
            var count = whereIdentifier.Count();
            if (count > 0)
                return true;
            return false;
        }
        public TagBasic(string displayName, ColorsScheme TagColor)
        {
            DisplayName = displayName;
            identifier = new TagRef(Random.Shared.NextInt64());
            TagColors = TagColor;
            TagManager.Instance.setTag(Identifier,this);
        }
        public TagBasic(string displayName) : this(displayName, ColorsScheme.AccentColors) { }
        public static TagBasic createRandom()
        {
            return new TagBasic("tag" + Random.Shared.Next(100), ColorsScheme.AccentColors);
        }

        public Tag Clone()
        {
            return this;
        }
    }
}