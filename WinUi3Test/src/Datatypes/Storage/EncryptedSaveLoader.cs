using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public class EncryptedSaveLoader : SaveLoader
{
    [JsonRequired]
    private SaveLoader saveLoader;
    [JsonIgnore]
    private string key;

    public bool KeyEmpty => key == null;

    [JsonConstructor]
    public EncryptedSaveLoader(SaveLoader parent)
    {
        saveLoader = parent;
    }
    public EncryptedSaveLoader(SaveLoader parent, string key) : this(parent)
    {
        this.key = key;
    }
    public bool Save(byte[] data)
    {
        return saveLoader.Save(data.EncryptAes(key));
    }

    public byte[] Load()
    {
        return saveLoader.Load().DecryptAes(key);
    }

    public DateTime LastAccessTime => saveLoader.LastAccessTime;

    public bool IsValid()
    {
        return saveLoader.IsValid();
    }

    public string DisplayPath => saveLoader.DisplayPath;
}
