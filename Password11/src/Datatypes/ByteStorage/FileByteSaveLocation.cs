using System;
using System.IO;
using Newtonsoft.Json;

namespace Password11.Datatypes;

public class FileByteLocation : ByteSaveLocation, LocationDisplayModel
{
    public FileByteLocation(string filePath)
    {
        Path = filePath;
    }

    [JsonIgnore]
    public string Path
    {
        get
        {
            if (DisplayPath == null)
                RequestPath();
            return DisplayPath;
        }
        set => DisplayPath = value;
    }

    public bool Save(byte[] data)
    {
        try
        {
            File.WriteAllBytes(DisplayPath, data);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public byte[] Load()
    {
        return File.ReadAllBytes(DisplayPath);
    }

    public bool IsValid()
    {
        return File.Exists(DisplayPath);
    }

    [JsonIgnore] public LocationDisplayModel Model => this;

    [JsonIgnore] [field: JsonRequired] public string DisplayPath { get; private set; }

    public string DisplayName => System.IO.Path.GetFileNameWithoutExtension(Path);

    [JsonIgnore] public DateTime LastAccessTime => File.GetLastAccessTime(DisplayPath);

    private void RequestPath()
    {
    }
}