using System;
using System.Linq;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Password11.src.Ui;
using Password11.src.Util;

namespace Password11.Datatypes;

public class TagBasic : Tag
{
    [JsonRequired]
    public string DisplayName { get; set; }
    [JsonRequired]
    public UniqueId<Tag> Identifier { get; set; }
    [JsonRequired]
    public string TagColorsString { get; set; }
    [JsonIgnore]
    public ColorsScheme TagColors
    {
        get => ColorsScheme.FromString(TagColorsString);
        set => TagColorsString = value.ToString();
    }

    [JsonIgnore]
    public Brush BaseColorBrush => TagColors.BaseColor.asBrush;
    [JsonIgnore]
    public Brush HoverColorBrush => TagColors.HoverColor.asBrush;
    [JsonIgnore]
    public Brush SymbolColorBrush => TagColors.SymbolColor.asBrush;

    public bool matches(Taggable account)
    {
        var r = new { Identifier };
        var whereIdentifier = account.Tags.Where(a => a.id == r.Identifier.id).ToList();
        var count = whereIdentifier.Count();
        if (count > 0)
            return true;
        return false;
    }
    public TagBasic(string displayName, ColorsScheme tagColor) : this(displayName,tagColor.ToString())
    {
    }
    public TagBasic(string displayName) : this(displayName, ColorsScheme.AccentColors) { }

    public TagBasic() : this(String.Empty) { }

    private TagBasic(string displayName, string tagColorsString)
    {
        DisplayName = displayName;
        Identifier = new UniqueId<Tag>(Random.Shared.NextInt64());
        TagColorsString = tagColorsString;
    }

    public Tag CloneRef()
    {
        return new TagBasic(DisplayName.Clone() as string, TagColorsString);
    }

    public void Restore(Tag state)
    {
        var stateBasic = (TagBasic)state;
        DisplayName = stateBasic.DisplayName;
        TagColorsString = stateBasic.TagColorsString;
    }

}