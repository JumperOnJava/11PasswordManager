using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public class EncryptedByteLocationDisplay : ByteSaveLocation, LocationDisplayModel
{
    [JsonRequired]
    private ByteSaveLocation byteSaveLocation;
    [JsonIgnore]
    private string key;

    public bool KeyEmpty => key == null;

    [JsonConstructor]
    public EncryptedByteLocationDisplay(ByteSaveLocation parent)
    {
        byteSaveLocation = parent;
    }
    public EncryptedByteLocationDisplay(ByteSaveLocation parent, string key) : this(parent)
    {
        this.key = key;
    }
    public bool Save(byte[] data)
    {
        return byteSaveLocation.Save(data.EncryptAes(key));
    }

    public byte[] Load()
    {
        return byteSaveLocation.Load().DecryptAes(key);
    }

    public DateTime LastAccessTime => byteSaveLocation.Model.LastAccessTime;

    public LocationDisplayModel Model => this;
    public bool IsValid()
    {
        return byteSaveLocation.IsValid();
    }


    public string DisplayPath => byteSaveLocation.Model.DisplayPath;
}
