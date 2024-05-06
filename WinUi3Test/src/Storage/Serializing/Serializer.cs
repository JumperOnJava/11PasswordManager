using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using WinUi3Test.src.Storage;

namespace WinUi3Test;

public class TypeMappingConverter<T, TImplementation> : JsonConverter<T>
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
