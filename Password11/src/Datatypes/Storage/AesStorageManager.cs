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
    [JsonRequired] private StorageManager target { get; set; }

    public AesStorageManager(StorageManager target, string key) : this(target)
    {
        this.key = key;
        this.target = target;
    }

    [JsonConstructor]
    private AesStorageManager(StorageManager target)
    {
        this.target = target;
        if (target is KeyReceiver krc)
        {
            krc.KeyGetter = () =>
            {
                return this.key;
            };
        }
    }
    public async Task<StorageData> GetData()
    {
        try
        {
            var decryptedData = (await target.GetData()).CloneRef();
            decryptedData.Tags = decryptedData.Tags.Select(tag =>
            {
                tag.DisplayName = tag.DisplayName.DecodeBase64().DecryptAes(key).DecodeUtf8();
                ;
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
            this.key = null;
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
        target.SetData(encryptedData);
        return Task.CompletedTask;
    }

    public bool IsValid()
    {
        return target.IsValid();
    }

    [JsonIgnore] public LocationDisplayModel DisplayInfo => target.DisplayInfo ;
    public async Task<bool> SetupManagerInGui(Page parent)
    {
        if (await target.SetupManagerInGui(parent))
        {
            if (key == null)
            {
                var r = await PasswordDialog.AskPassword(parent, false,title:"Enter encryption key").GetResult();
                if (r.Item1)
                {
                    key = r.Item2;
                    var src = new TaskCompletionSource<bool>();
                    ExceptionDialog.ShowExceptionOnFail(parent, async () =>
                    {
                        try
                        {
                            (await GetData()).CloneRef();
                        }
                        catch (Exception e)
                        {
                            src.SetResult(false);
                            throw e;
                        }
                        src.SetResult(true);
                    });
                    return await src.Task;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public void Fail()
    {
        this.key = null;
    }
}

internal interface KeyReceiver
{
    Func<string> KeyGetter { set; }
}

public static class AesStorageExtension
{
    public static StorageManager AesEncryptedManager(this StorageManager manager, string key)
    {
        return new AesStorageManager(manager,key);
    }
}