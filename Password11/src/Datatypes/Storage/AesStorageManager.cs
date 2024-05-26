using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Password11.Datatypes;
using Password11.Dialogs;

namespace Password11.src.Util;

public class AesStorageManager : StorageManager
{
    [JsonIgnore] private string key;

    public AesStorageManager(StorageManager target, string key) : this(target)
    {
        this.key = key;
        this.target = target;
    }

    [JsonConstructor]
    private AesStorageManager(StorageManager target)
    {
        this.target = target;
    }

    [JsonRequired] private StorageManager target { get; }

    public async Task<StorageData> GetData()
    {
        try
        {
            var decryptedData = (await target.GetData()).CloneRef();
            decryptedData.Tags = decryptedData.Tags.Select(tag =>
            {
                tag.DisplayName = tag.DisplayName.DecodeBase64().DecryptAes(key).DecodeUtf8();
                tag.TagColorsString = tag.TagColorsString.DecodeBase64().DecryptAes(key).DecodeUtf8();
                return tag;
            }).ToList();
            decryptedData.Accounts = decryptedData.Accounts.Select(account =>
            {
                account.Fields = account.Fields.ToList().Select(kvp =>
                {
                    kvp.Value.Name = kvp.Value.Name.DecodeBase64().DecryptAes(key).DecodeUtf8();
                    kvp.Value.Data = kvp.Value.Data.DecodeBase64().DecryptAes(key).DecodeUtf8();
                    return kvp.Value;
                }).ToDictionary(k => k.Name, k => k);
                return account;
            }).ToList();
            return decryptedData;
        }
        catch (Exception e)
        {
            key = null;
            throw new Exception("Wrong key or data", e);
        }
    }

    public Task SetData(StorageData value)
    {
        var encryptedData = value.CloneRef();
        encryptedData.Tags = encryptedData.Tags.Select(tag =>
        {
            tag.DisplayName = tag.DisplayName.EncodeUtf8().EncryptAes(key).EncodeBase64();
            tag.TagColorsString = tag.TagColorsString.EncodeUtf8().EncryptAes(key).EncodeBase64();
            return tag;
        }).ToList();
        encryptedData.Accounts = encryptedData.Accounts.Select(Account =>
        {
            Account.Fields = Account.Fields.ToList().Select(kvp =>
            {
                kvp.Value.Name = kvp.Value.Name.EncodeUtf8().EncryptAes(key).EncodeBase64();
                kvp.Value.Data = kvp.Value.Data.EncodeUtf8().EncryptAes(key).EncodeBase64();
                return kvp.Value;
            }).ToDictionary(k => k.Name, k => k);
            return Account;
        }).ToList();
        return target.SetData(encryptedData);
    }

    public bool IsValid()
    {
        return target.IsValid();
    }

    [JsonIgnore] public LocationDisplayModel DisplayInfo => target.DisplayInfo;

    public async Task<bool> SetupManagerInGui(Page parent)
    {
        if (!await target.SetupManagerInGui(parent)) return false;
        key = null;
        var r = await PasswordDialog.AskPassword(parent, false, "Enter encryption key").GetResult();
        if (!r.Item1) return false;

        key = r.Item2;
        bool result;
        try
        {
            (await GetData()).CloneRef();
            result = true;
        }
        catch (Exception e)
        {
            await ExceptionDialog.ShowException(parent, e);
            result = false;
        }

        return result;
    }

    public void ResetOnFail()
    {
        key = null;
        target.ResetOnFail();
    }
}


public static class AesStorageExtension
{
    public static StorageManager AesEncryptedManager(this StorageManager manager, string key)
    {
        return new AesStorageManager(manager, key);
    }
}