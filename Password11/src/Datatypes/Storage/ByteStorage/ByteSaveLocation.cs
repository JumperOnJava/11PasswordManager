using System;
using System.Text;
using System.Threading.Tasks;
using Password11.src.Util;

namespace Password11.Datatypes;

public interface ByteSaveLocation
{
    public bool Save(byte[] data);
    public byte[] Load();
    public LocationDisplayModel Model { get; }
    public bool IsValid();
}

public interface LocationDisplayModel
{
    public DateTime LastAccessTime { get; }
    public string DisplayPath { get; }
    public string DisplayName { get; }
}