using System;
using Windows.UI;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Password11.ColorLib;
using Password11.Datatypes;
using Password11.Datatypes.Serializing;
using Password11.src.Util;
using Password11Lib.Util;

namespace Password11.ViewModel;

public class UiTag : PropertyChangable, RefClonable<UiTag>, Identifiable<Tag>
{
    private bool selected;

    public UiTag(Tag target)
    {
        Target = target;
    }

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            onPropertyChanged();
        }
    }

    public Tag Target { get; set; }

    public string DisplayName
    {
        get => Target.DisplayName;
        set
        {
            if (Target.DisplayName != value) TextChanged?.Invoke(value);
            Target.DisplayName = value;
            onPropertyChanged();
        }
    }

    public UiTag Self => this;

    public ColorsScheme TagColors
    {
        get => Target.TagColors;
        set
        {
            Target.TagColors = value;
            onPropertyChanged("TagColor");
            onPropertyChanged("baseColor");
            onPropertyChanged("hoverColor");
            onPropertyChanged("symbolColor");
        }
    }

    [JsonIgnore] public Color baseColor => TagColors.BaseColor.AsWinColor;

    [JsonIgnore] public SolidColorBrush baseColorBrush => new(TagColors.BaseColor.AsWinColor);

    [JsonIgnore] public Color hoverColor => TagColors.HoverColor.AsWinColor;

    [JsonIgnore] public Color symbolColor => TagColors.SymbolColor.AsWinColor;

    [JsonIgnore] public Brush BaseColorBrush => Target.BaseColorBrush;

    [JsonIgnore] public Brush HoverColorBrush => Target.HoverColorBrush;

    [JsonIgnore] public Brush SymbolColorBrush => Target.SymbolColorBrush;

    public UniqueId<Tag> Identifier => Target.Identifier;

    public UiTag CloneRef()
    {
        return new UiTag(Target.CloneRef());
    }

    public void Restore(UiTag state)
    {
        Target = state.Target;
    }

    public event Action<string> TextChanged;

    public bool matches(Taggable tag)
    {
        return Target.matches(tag);
    }
}