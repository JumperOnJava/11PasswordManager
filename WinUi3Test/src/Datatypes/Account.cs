using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Windows.UI;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public interface Account : Taggable, Clonable<Account>, Identifiable
{
    public string TargetApp { get; set; }
    public string DisplayName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Dictionary<string, FieldData> AdditionalData { get; set; }
    [Newtonsoft.Json.JsonIgnore]
    public Type AccountEditor => typeof(PasswordEdit);
    public ColorsScheme Colors { get; set; }
    public Color BaseColorBindable { get; set; }
    string AppLink { get; set; }
}

public class FieldData
{
    public bool IsHidden;
    public string Data;
    public FieldData(bool isHidden, string data)
    {
        IsHidden = isHidden;
        Data = data;
    }
}