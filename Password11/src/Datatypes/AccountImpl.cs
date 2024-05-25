using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Newtonsoft.Json;
using Password11.src.Ui;
using Password11.src.Util;
using Password11Lib.Util;

namespace Password11.Datatypes;

public class AccountImpl : Account
{
    public AccountImpl()
    {
        Identifier = UniqueId<Account>.CreateRandom<Account>();
    }

    [JsonRequired] public UniqueId<Account> Identifier { get; set; }
    [JsonRequired] public List<UniqueId<Tag>> Tags { get; set; } = new();
    [JsonRequired] public Dictionary<string, FieldData> Fields { get; set; } = new();

    [JsonIgnore]
    public string TargetApp
    {
        get => GetFieldData(nameof(TargetApp));
        set => SetFieldData(nameof(TargetApp), value);
    }

    [JsonIgnore]
    public string DisplayName
    {
        get => GetFieldData(nameof(DisplayName));
        set => SetFieldData(nameof(DisplayName), value);
    }

    [JsonIgnore]
    public string Username
    {
        get => GetFieldData(nameof(Username));
        set => SetFieldData(nameof(Username), value);
    }

    [JsonIgnore]
    public string Email
    {
        get => GetFieldData(nameof(Email));
        set => SetFieldData(nameof(Email), value);
    }

    [JsonIgnore]
    public string Password
    {
        get => GetFieldData(nameof(Password));
        set => SetFieldData(nameof(Password), value);
    }

    [JsonIgnore]
    public string AppLink
    {
        get => GetFieldData(nameof(AppLink));
        set => SetFieldData(nameof(AppLink), value);
    }

    [JsonIgnore]
    public ColorsScheme Colors
    {
        get => ColorsScheme.FromString(GetFieldData(nameof(Colors)));
        set => SetFieldData(nameof(Colors), value.ToString());
    }

    [JsonIgnore]
    public Color BaseColorBindable
    {
        get => Colors.BaseColor.asWinColor;
        set => Colors = new ColorsScheme(new AdvColor(value));
    }

    private string GetFieldData(string fieldName)
    {
        if (Fields.TryGetValue(fieldName, out FieldData fieldData))
        {
            return fieldData.Data;
        }

        return string.Empty;
    }

    private void SetFieldData(string fieldName, string data)
    {
        if (Fields.ContainsKey(fieldName))
        {
            Fields[fieldName].Data = data;
        }
        else
        {
            Fields[fieldName] = new FieldData(false,fieldName,String.Empty, true);
        }
    }

    public Account CloneRef()
    {
        var account = new AccountImpl();
        account.Fields = new Dictionary<string, FieldData>(Fields)
            .Select(kvp=>new KeyValuePair<string,FieldData>(kvp.Key,kvp.Value.CloneRef()))
            .ToDictionary(k=>k.Key,k=>k.Value);;
        account.Tags = new List<UniqueId<Tag>>(Tags);
        account.Identifier = Identifier;
        return account;
    }

    public void Restore(Account state)
    {
        Identifier = state.Identifier;
        Fields = state.Fields;
        Tags = state.Tags;
    }
}