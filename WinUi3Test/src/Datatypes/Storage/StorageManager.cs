using System;
using System.Threading.Tasks;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public class StorageManager
{
    public SaveLoader DataLoader;
    public StorageData Data;

    public StorageManager(SaveLoader dataLoader)
    {
        this.DataLoader = dataLoader; 
        UpdateFromStorage();
    }

    public StorageManager(SaveLoader dataLoader, StorageData data)
    {
        this.DataLoader = dataLoader;
        this.Data = data;
        SaveToStorage();
        UpdateFromStorage();
    }
    public void UpdateFromStorage()
    {
        var data = DataLoader.Load();
        var text = data.DecodeUtf8();
        this.Data = JsonTools.Deserialize<StorageData>(text);
    }

    public bool SaveToStorage()
    {
        return DataLoader.Save(JsonTools.Serialize(Data).EncodeUtf8());
    }
}