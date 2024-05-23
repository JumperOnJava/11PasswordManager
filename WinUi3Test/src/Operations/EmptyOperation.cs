namespace WinUi3Test.Datatypes;

public class EmptyOperation<T> : Operation<T>
{

    /// <summary>
    /// Passes default(<typeparamref name="T"/>) as target object
    /// Useful when want create 
    /// </summary>
    public EmptyOperation() : base(()=>default){}

}