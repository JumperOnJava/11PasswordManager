namespace Password11.Datatypes;

/// <summary>
///     Same as Operation<typeparamref name="T" /> but allows creation of new objects of <typeparamref name="T" />
/// </summary>
/// <typeparam name="T"></typeparam>
public class CreationOperation<T> : Operation<T> where T : new()
{
    /// <summary>
    ///     Passes new <typeparamref name="T" /> object as operation target
    ///     Useful to create new objects with parameterless constructor
    /// </summary>
    public CreationOperation() : base(() => new T())
    {
    }
}