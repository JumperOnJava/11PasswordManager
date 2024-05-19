using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public class TagRef : Tag
{
    [JsonRequired]
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
    [Newtonsoft.Json.JsonIgnore]
    public ColorsScheme TagColors
    {
        get => TagManager.Instance.getTag(this).TagColors; set
        {
            var tag = TagManager.Instance.getTag(this);
            tag.TagColors = value;
            TagManager.Instance.setTag(this, tag);
        }
    }
    [Newtonsoft.Json.JsonIgnore]
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

    [Newtonsoft.Json.JsonIgnore]
    public Brush BaseColorBrush => TagManager.Instance.getTag(this).BaseColorBrush;

    [Newtonsoft.Json.JsonIgnore]
    public Brush HoverColorBrush => TagManager.Instance.getTag(this).HoverColorBrush;

    [Newtonsoft.Json.JsonIgnore]
    public Brush SymbolColorBrush => TagManager.Instance.getTag(this).SymbolColorBrush;

    [Newtonsoft.Json.JsonIgnore]
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
    [Newtonsoft.Json.JsonIgnore]
    public Brush BaseColorBrush { get; }
    [Newtonsoft.Json.JsonIgnore]
    public Brush HoverColorBrush { get; }
    [Newtonsoft.Json.JsonIgnore]
    public Brush SymbolColorBrush { get; }
}
public struct TagBasic : Tag
{
    public string DisplayName { get; set; }
    public TagRef Identifier { get; set; }
    public ColorsScheme TagColors { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public Brush BaseColorBrush => TagColors.BaseColor.asBrush;
    [Newtonsoft.Json.JsonIgnore]
    public Brush HoverColorBrush => TagColors.HoverColor.asBrush;
    [Newtonsoft.Json.JsonIgnore]
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
    public TagBasic(string displayName, ColorsScheme tagColor)
    {
        DisplayName = displayName;
        Identifier = new TagRef(Random.Shared.NextInt64());
        TagColors = tagColor;
        TagManager.Instance.setTag(Identifier,this);
    }
    public TagBasic(string displayName) : this(displayName, ColorsScheme.AccentColors) { }

    public TagBasic() : this(String.Empty) { }

    public Tag Clone()
    {
        return this;
    }
}