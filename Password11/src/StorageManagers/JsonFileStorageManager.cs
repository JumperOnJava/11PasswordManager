using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Datatypes.Serializing;
using Password11.src.Util;
using static System.Net.Mime.MediaTypeNames;

namespace Password11.StorageManagers;

public class JsonFileStorageManager : StorageManager
{
    [JsonInclude] public ByteSaveLocation DataLoader;

    public JsonFileStorageManager(ByteSaveLocation dataLoader)
    {
        DataLoader = dataLoader;
    }

    private JsonFileStorageManager()
    {
    }

    public JsonFileStorageManager(ByteSaveLocation dataLoader, StorageData data) : this(dataLoader)
    {
        SetData(data);
    }

    public bool IsValid()
    {
        return DataLoader.IsValid();
    }

    [Newtonsoft.Json.JsonIgnore] public LocationDisplayModel DisplayInfo => DataLoader.Model;

    public async Task<bool> SetupManagerInGui(Page parent)
    {
        return true;
    }

    public void ResetOnFail()
    {
    }

    public async Task<StorageData> GetData()
    {
        return JsonTools.DeserializeSmart<StorageData>(DataLoader.Load().DecodeUtf8());
    }

    public async Task SetData(StorageData value)
    {
        DataLoader.Save(JsonTools.SerializeSmart(value).EncodeUtf8());
    }

    public static JsonFileStorageManager CreateNew(ByteSaveLocation dataloader)
    {
        var manager = new JsonFileStorageManager();
        manager.DataLoader = dataloader;
        manager.SetData(new StorageData());
        return manager;
    }
}