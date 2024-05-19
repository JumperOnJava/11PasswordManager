using System;
using System.Text;
using System.Text.Json;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes.Serializing;

public class Test
{
    public static void Start()
    {
        var manager = new StorageManager(new FileSaveLoader("selftest").AesEncryptedStorage(Random.Shared.NextInt64().ToString()),new StorageData());
        if (manager.Data != null)
            Console.WriteLine("Encoding test passed!");
        else
            throw new Exception("Encoding test failed");
        //Environment.Exit(0);
    }
}