using System;
using System.Collections.Generic;
using Windows.UI;
using Newtonsoft.Json;
using Password11.Datatypes.Serializing;
using Password11.src.Ui;
using Password11Lib.Util;

namespace Password11.Datatypes;

public interface Account : Taggable, RefClonable<Account>, Identifiable<Account>
{
    public Dictionary<string, FieldData> Fields { get; set; }
    public string TargetApp { get; set; }
    public string DisplayName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    string AppLink { get; set; }
    [JsonIgnore] public Type AccountEditor => typeof(AccountEdit);
    public ColorsScheme Colors { get; set; }
    public Color BaseColorBindable { get; set; }
}

public class FieldData : Identifiable<FieldData>, RefClonable<FieldData>
{
    [JsonRequired] public string Data;
    [JsonRequired] public bool IsHidden;
    [JsonRequired] public string Name;
    [JsonRequired] public bool Official;

    public FieldData(bool isHidden, string name, string data, bool official) : this(isHidden, name, data, official,
        UniqueId<FieldData>.CreateRandom<FieldData>())
    {
    }

    [JsonConstructor]
    private FieldData(bool isHidden, string name, string data, bool official, UniqueId<FieldData> id)
    {
        IsHidden = isHidden;
        Name = name;
        Data = data;
        Official = official;
        Identifier = id;
    }

    [JsonRequired] public UniqueId<FieldData> Identifier { get; set; }

    public FieldData CloneRef()
    {
        return new FieldData(IsHidden, Name, Data, Official, Identifier);
    }

    public void Restore(FieldData state)
    {
        Identifier = state.Identifier;
        Official = state.Official;
        Data = state.Data;
        IsHidden = state.IsHidden;
        Name = state.Name;
    }
}