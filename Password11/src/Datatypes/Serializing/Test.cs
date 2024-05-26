using System;

namespace Password11.Datatypes.Serializing;

public class Test
{
    private static readonly bool testEncryption = false;

    public static void Start()
    {
        if (testEncryption)
        {
            var key = Random.Shared.NextInt64().ToString();
            var manager = new JsonFileStorageManager(new FileByteLocation("selftest"), new StorageData());
            if (manager.GetData() != null)
                Console.WriteLine("Encoding test passed!");
            else
                throw new Exception("Encoding test failed");
        }
    }
}