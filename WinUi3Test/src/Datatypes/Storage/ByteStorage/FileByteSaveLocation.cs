using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WinUi3Test.Datatypes;

public class FileByteLocation : ByteSaveLocation, LocationDisplayModel
{
    [JsonRequired]
    private string path;

    public FileByteLocation(string filePath)
    {
        this.Path = filePath;
    }

    [JsonIgnore]
    public string Path
    {
        get
        {
            if (path == null)
                RequestPath();
            return path;
        }
        set => path = value;
    }

    [JsonIgnore]
    public string DisplayPath => System.IO.Path.GetFileNameWithoutExtension(Path);
    private void RequestPath()
    {
        
    }

    public bool Save(byte[] data)
    {
        try
        {
            File.WriteAllBytes(path, data);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public byte[] Load()
    {
        return File.ReadAllBytes(path);
    }

    [JsonIgnore]
    public DateTime LastAccessTime => File.GetLastAccessTime(path);
    public bool IsValid() => File.Exists(path);
    
    [JsonIgnore]
    public LocationDisplayModel Model => this;
}