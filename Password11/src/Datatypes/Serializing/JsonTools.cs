namespace Password11.Datatypes.Serializing;
using Newtonsoft.Json;
public class JsonTools
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        PreserveReferencesHandling = PreserveReferencesHandling.All
    };
    public static string Serialize(object value)
    {
        return JsonConvert.SerializeObject(value, Formatting.Indented, Settings);
    }
    public static T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }
}