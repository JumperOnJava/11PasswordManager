using System.Linq;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Password11.ColorLib;
using Password11Lib.Util;

namespace Password11.Datatypes;

public class TagBasic : Tag
{
    public TagBasic(string displayName, ColorsScheme tagColor) : this(displayName, tagColor.ToString())
    {
    }

    public TagBasic(string displayName) : this(displayName, ColorsScheme.AccentColors)
    {
    }

    public TagBasic() : this(string.Empty)
    {
    }

    private TagBasic(string displayName, string tagColorsString)
    {
        DisplayName = displayName;
        Identifier = UniqueId<Tag>.CreateRandom<Tag>();
        TagColorsString = tagColorsString;
    }

    [JsonRequired] public string DisplayName { get; set; }

    [JsonRequired] public UniqueId<Tag> Identifier { get; set; }

    [JsonRequired] public string TagColorsString { get; set; }

    [JsonIgnore]
    public ColorsScheme TagColors
    {
        get => ColorsScheme.FromString(TagColorsString);
        set => TagColorsString = value.ToString();
    }

    [JsonIgnore] public Brush BaseColorBrush => TagColors.BaseColor.AsBrush;

    [JsonIgnore] public Brush HoverColorBrush => TagColors.HoverColor.AsBrush;

    [JsonIgnore] public Brush SymbolColorBrush => TagColors.SymbolColor.AsBrush;

    public bool matches(Taggable account)
    {
        var r = new { Identifier };
        var whereIdentifier = account.Tags.Where(a => a.id == r.Identifier.id).ToList();
        var count = whereIdentifier.Count();
        if (count > 0)
            return true;
        return false;
    }

    public Tag CloneRef()
    {
        var tag = new TagBasic(DisplayName.Clone() as string, TagColorsString);
        tag.Identifier = Identifier;
        return tag;
    }

    public void Restore(Tag state)
    {
        var stateBasic = (TagBasic)state;
        DisplayName = stateBasic.DisplayName;
        TagColorsString = stateBasic.TagColorsString;
        Identifier = stateBasic.Identifier;
    }

    public override string ToString()
    {
        return $"{DisplayName}:{Identifier}";
    }
}