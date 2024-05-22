using System;
using System.Text;
using System.Threading.Tasks;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

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
}