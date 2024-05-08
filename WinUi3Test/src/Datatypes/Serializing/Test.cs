using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Util;

namespace WinUi3Test;

public class Test
{
    public static JsonSerializerOptions JsonOption =  new JsonSerializerOptions
    {
        IgnoreReadOnlyProperties = true,
        WriteIndented = true,
        Converters =
        {
            new TypeMappingConverter<Tag, TagBasic>(),
            new TypeMappingConverter<Account, AccountImpl>()
        }
    };

    public static void Start()
    {
        var text = "Encoding test";
        var key = "secret";
        var enc = Encryption.Encrypt(text, key);
        var dec = Encryption.Decrypt(enc, key);
        if (dec == text)
            Console.WriteLine("Encoding test passed!");
        else
            throw new Exception("Encoding test failed");
    }
}