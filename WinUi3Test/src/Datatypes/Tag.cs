using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Media;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public class UniqueTagId : UniqueId, Tag
{
    public UniqueTagId(long id) : base(id)
    {
    }

    public override int GetHashCode()
    {
        return (int)id;
    }
    public static bool operator ==(UniqueTagId a, UniqueTagId b)
    {
        return a.id == b.id;
    }
    public static bool operator !=(UniqueTagId a, UniqueTagId b)
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

    public Tag Clone() => Clone();

    [Newtonsoft.Json.JsonIgnore]
    public UniqueTagId IdentifierTagId => this;
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

    public Tag getTag(UniqueTagId uniqueTagId)
    {
        return tags[uniqueTagId.id];
    }

    public void setTag(UniqueTagId uniqueTagId, Tag tag)
    {
        tags[uniqueTagId.id] = tag;
    }
}

public interface Tag : Identifiable, Clonable<Tag>
{
    ColorsScheme TagColors { get; set; }
    string DisplayName { get; set; }
    public UniqueTagId IdentifierTagId { get;}
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
    [Newtonsoft.Json.JsonIgnore]
    public UniqueId Identifier => IdentifierTagId;
    public UniqueTagId IdentifierTagId { get; set; }
    public ColorsScheme TagColors { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public Brush BaseColorBrush => TagColors.BaseColor.asBrush;
    [Newtonsoft.Json.JsonIgnore]
    public Brush HoverColorBrush => TagColors.HoverColor.asBrush;
    [Newtonsoft.Json.JsonIgnore]
    public Brush SymbolColorBrush => TagColors.SymbolColor.asBrush;

    public bool matches(Taggable account)
    {
        var r = new { this.Identifier };
        var whereIdentifier = account.Tags.Where((a) => a.Identifier.id == r.Identifier.id).ToList();
        var count = whereIdentifier.Count();
        if (count > 0)
            return true;
        return false;
    }
    public TagBasic(string displayName, ColorsScheme tagColor)
    {
        DisplayName = displayName;
        IdentifierTagId = new UniqueTagId(Random.Shared.NextInt64());
        TagColors = tagColor;
        TagManager.Instance.setTag((UniqueTagId)Identifier, this);
    }
    public TagBasic(string displayName) : this(displayName, ColorsScheme.AccentColors) { }

    public TagBasic() : this(String.Empty) { }

    public Tag Clone()
    {
        return this;
    }

}