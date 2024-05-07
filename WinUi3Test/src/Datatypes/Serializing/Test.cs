using System;
using System.Collections.Generic;
using System.Text.Json;
using WinUi3Test.src.Storage;

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
        //JsonOption =

    }
}