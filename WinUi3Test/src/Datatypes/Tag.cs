using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;


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