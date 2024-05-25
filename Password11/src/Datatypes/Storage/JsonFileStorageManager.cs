using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes.Serializing;
using Password11.src.Util;
using Password11.StorageDialogs.FileStorage;

namespace Password11.Datatypes;

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

    public async Task<StorageData> GetData()
    {
        Restore();
        return data;
    }

    public async Task SetData(StorageData value)
    {
        DataLoader.Save(JsonTools.SerializeSmart(value).EncodeUtf8());
        Restore();
    }

    private void Restore()
    {
        var bytes = DataLoader.Load();
        var text = bytes.DecodeUtf8();
        this.data = JsonTools.DeserializeSmart<StorageData>(text);
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
        manager.SetData(new StorageData());
        return manager;
    }

    public JsonFileStorageManager(ByteSaveLocation dataLoader, StorageData data) : this(dataLoader)
    {
        this.SetData(data);
    }

}