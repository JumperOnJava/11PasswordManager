using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Password11.src.Util;

namespace Password11.Datatypes.Serializing;

public class Test
{
    private static bool testEncryption=false;

    public static void Start()
    {
        if(testEncryption){
            string key = Random.Shared.NextInt64().ToString();
            var manager = new JsonFileStorageManager(new FileByteLocation("selftest"),new StorageData());
            if (manager.GetData() != null)
                Console.WriteLine("Encoding test passed!");
            else
                throw new Exception("Encoding test failed");
        }
    }
}