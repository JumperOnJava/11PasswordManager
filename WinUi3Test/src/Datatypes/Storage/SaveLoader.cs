using System;
using System.Text;
using System.Threading.Tasks;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public interface SaveLoader
{
    public bool Save(byte[] data);
    public byte[] Load();
    public DateTime LastAccessTime { get; }
    public bool IsValid();
    public string DisplayPath { get; }
}