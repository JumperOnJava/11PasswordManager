using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.Util;
using WinUi3Test.StorageDialogs.FileStorage;

namespace WinUi3Test.Datatypes;

public class JsonFileStorageManager : StorageManager
{
    [JsonInclude]
    public ByteSaveLocation DataLoader;
    public bool IsValid() => DataLoader.IsValid();

    [Newtonsoft.Json.JsonIgnore]
    public LocationDisplayModel DisplayInfo => DataLoader.Model;

    public async Task<bool> SetupManagerInGui(Page parent)
    {
        return true;
    }

    public void Fail()
    {
        
    }

    private StorageData data;
    [Newtonsoft.Json.JsonIgnore]
    public StorageData Data
    {
        get
        {
            Restore();
            return data;
        }
        set
        {
            DataLoader.Save(JsonTools.Serialize(value).EncodeUtf8());
            Restore();
        }
    }

    private void Restore()
    {
        var bytes = DataLoader.Load();
        var text = bytes.DecodeUtf8();
        this.data = JsonTools.Deserialize<StorageData>(text);
    }
    public JsonFileStorageManager(ByteSaveLocation dataLoader)
    {
        this.DataLoader = dataLoader;
        Restore();
    }
    private JsonFileStorageManager(){}
    public static JsonFileStorageManager CreateNew(ByteSaveLocation dataloader)
    {
        var manager = new JsonFileStorageManager();
        manager.DataLoader = dataloader;
        manager.Data = new StorageData();
        return manager;
    }

    public JsonFileStorageManager(ByteSaveLocation dataLoader, StorageData data) : this(dataLoader)
    {
        this.Data = data;
    }

}

public interface StorageManager
{
    StorageData Data { get; set; }
    public bool IsValid();
    LocationDisplayModel DisplayInfo { get; }
    Task<bool> SetupManagerInGui(Page parent);
    void Fail();
}