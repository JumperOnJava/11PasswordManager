using System;

namespace Password11.Datatypes;

public interface ByteSaveLocation
{
    public LocationDisplayModel Model { get; }
    public bool Save(byte[] data);
    public byte[] Load();
    public bool IsValid();
}

public interface LocationDisplayModel
{
    public DateTime LastAccessTime { get; }
    public string DisplayPath { get; }
    public string DisplayName { get; }
}