using Newtonsoft.Json;

namespace Password11Lib.Util;

public interface Identifiable<T>
{
    [JsonRequired] public UniqueId<T> Identifier { get; }
}

public class UniqueId<T>
{
    [JsonRequired] public readonly long id;

    public UniqueId(long id)
    {
        this.id = id;
    }
    
    public static UniqueId<T> CreateRandom<T>()
    {
        return new UniqueId<T>(Random.Shared.NextInt64());
    }

    public override string ToString()
    {
        return id.ToString();
    }

    public override bool Equals(object? other)
    {
        return id == (other as UniqueId<T>)?.id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}