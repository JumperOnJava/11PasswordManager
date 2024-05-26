using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Password11.Datatypes.Serializing;
using Password11.src.Ui;
using Password11Lib.Util;

namespace Password11.Datatypes;

public interface Tag : Identifiable<Tag>, RefClonable<Tag>
{
    ColorsScheme TagColors { get; set; }
    string TagColorsString { get; set; }
    string DisplayName { get; set; }

    [JsonIgnore] public Brush BaseColorBrush { get; }

    [JsonIgnore] public Brush HoverColorBrush { get; }

    [JsonIgnore] public Brush SymbolColorBrush { get; }

    bool matches(Taggable tag);
}