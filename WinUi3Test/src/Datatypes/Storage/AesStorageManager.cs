using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using WinUi3Test.Datatypes;
using WinUi3Test.Dialogs;

namespace WinUi3Test.src.Util;

internal class AesStorageManager : StorageManager
{
    [JsonIgnore]
    private string key;   
    
    [JsonRequired]
    private StorageManager target { get; }

    public AesStorageManager(StorageManager target, string key)
    {
        this.key = key;
        this.target = target;
    }

    [JsonIgnore]
    public StorageData Data
    {
        get
        {
            try
            {
                var decryptedData = target.Data.CloneRef();
                decryptedData.Tags = decryptedData.Tags.Select(kvp =>
                {
                    kvp.Value.DisplayName = kvp.Value.DisplayName.DecodeBase64().DecryptAes(key).DecodeUtf8();
                    ;
                    kvp.Value.TagColorsString = kvp.Value.TagColorsString.DecodeBase64().DecryptAes(key).DecodeUtf8();
                    return kvp;
                }).ToDictionary(k => k.Key, k => k.Value);
                decryptedData.Accounts = decryptedData.Accounts.Select(kvp =>
                {
                    kvp.Value.Fields = kvp.Value.Fields.ToList().Select(kvp =>
                    {
                        kvp.Value.Name = kvp.Value.Name.DecodeBase64().DecryptAes(key).DecodeUtf8();
                        kvp.Value.Data = kvp.Value.Data.DecodeBase64().DecryptAes(key).DecodeUtf8();
                        return kvp.Value;
                    }).ToDictionary(k => k.Name, k => k);
                    return kvp;
                }).ToDictionary(k => k.Key, k => k.Value);
                return decryptedData;
            }
            catch (Exception e)
            {
                this.key = null;
                throw new Exception("Wrong key or data",e);
            }
        }
        set
        {
            var encryptedData = value.CloneRef();
            encryptedData.Tags = encryptedData.Tags.Select(kvp =>
            {
                kvp.Value.DisplayName = kvp.Value.DisplayName.EncodeUtf8().EncryptAes(key).EncodeBase64();
                kvp.Value.TagColorsString = kvp.Value.TagColorsString.EncodeUtf8().EncryptAes(key).EncodeBase64();
                return kvp;
            }).ToDictionary(k => k.Key, k => k.Value);
            encryptedData.Accounts = encryptedData.Accounts.Select(kvp =>
            {
                kvp.Value.Fields = kvp.Value.Fields.ToList().Select(kvp =>
                {
                    kvp.Value.Name = kvp.Value.Name.EncodeUtf8().EncryptAes(key).EncodeBase64();
                    kvp.Value.Data = kvp.Value.Data.EncodeUtf8().EncryptAes(key).EncodeBase64();
                    return kvp.Value;
                }).ToDictionary(k => k.Name, k => k);
                return kvp;
            }).ToDictionary(k => k.Key, k => k.Value);
            target.Data = encryptedData;
        }
    }

    public bool IsValid()
    {
        return target.IsValid();
    }

    public LocationDisplayModel DisplayInfo => target.DisplayInfo;
    public async Task<bool> SetupManagerInGui(Page parent)
    {
        if (await target.SetupManagerInGui(parent))
        {
            if (key == null)
            {
                var r = await PasswordDialog.AskPassword(parent, false).GetResult();

                if (r.Item1)
                {
                    key = r.Item2;
                    var src = new TaskCompletionSource<bool>();
                    Extensions.ShowExceptionOnFail(parent, () =>
                    {
                        try
                        {
                            Data.CloneRef();
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