namespace WinUi3Test.Datatypes.Serializing;

public interface RefClonable<T>
{
    T CloneRef();

    void Restore(T state);
}