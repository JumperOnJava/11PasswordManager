using System;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Password11.Datatypes.Serializing;

public class JsonTools
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto
    };

    private static readonly JsonSerializerOptions options = new()
    {
        IncludeFields = true,
        WriteIndented = true,
        Converters =
        {
            new TypeMappingConverter<Tag, TagBasic>(),
            new TypeMappingConverter<Account, AccountImpl>()
        }
    };

    public static string SerializeSmart(object value)
    {
        return JsonConvert.SerializeObject(value, Formatting.Indented, Settings);
    }

    public static T DeserializeSmart<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }

    public static string Serialize(object value)
    {
        return JsonSerializer.Serialize(value, value.GetType(), options);
    }

    public static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }
}

public class TypeMappingConverter<T, TImplementation> : System.Text.Json.Serialization.JsonConverter<T>
    where TImplementation : T
{
    public override T Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<TImplementation>(ref reader, options);
    }

    public override void Write(
        Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (TImplementation)value!, options);
    }
}