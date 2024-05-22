using System;
using System.Threading.Tasks;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public class ByteStorageManager : StorageManager
{
    private ByteSaveLocation DataLoader;
    public bool IsValid() => DataLoader.IsValid();
    public LocationDisplayModel DisplayInfo => DataLoader.Model;
    private StorageData data;
    public StorageData Data
    {
        get
        {
            Restore();
            return data;
        }
        set
        {
            this.data = value;
            DataLoader.Save(JsonTools.Serialize(Data).EncodeUtf8());
            Restore();
        }
    }

    private void Restore()
    {
        var bytes = DataLoader.Load();
        var text = bytes.DecodeUtf8();
        this.Data = JsonTools.Deserialize<StorageData>(text);
    }
    public ByteStorageManager(ByteSaveLocation dataLoader)
    {
        this.DataLoader = dataLoader;
        Restore();
    }

    public ByteStorageManager(ByteSaveLocation dataLoader, StorageData data) : this(dataLoader)
    {
        this.Data = data;
    }

}

public interface StorageManager
{
    StorageData Data { get; set; }
    public bool IsValid();
    LocationDisplayModel DisplayInfo { get; }
}