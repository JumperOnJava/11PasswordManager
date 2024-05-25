using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using Password11.Datatypes.Serializing;
using Password11.src.Ui;
using Password11.src.Util;
using Password11Lib.Util;

namespace Password11.Datatypes;


public interface Tag : Identifiable<Tag>, RefClonable<Tag>
{
    ColorsScheme TagColors { get; set; }
    string TagColorsString { get; set; }
    string DisplayName { get; set; }
    bool matches(Taggable tag);
    [Newtonsoft.Json.JsonIgnore]
    public Brush BaseColorBrush { get; }
    [Newtonsoft.Json.JsonIgnore]
    public Brush HoverColorBrush { get; }
    [Newtonsoft.Json.JsonIgnore]
    public Brush SymbolColorBrush { get; }
}