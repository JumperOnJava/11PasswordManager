namespace Password11.Datatypes.Serializing;

public interface RefClonable<T>
{
    T CloneRef();

    void Restore(T state);
}