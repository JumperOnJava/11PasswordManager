using System;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.Util;

namespace WinUi3Test.Datatypes;

public class Operation<T> : Operation where T : class, Clonable<T> 
{
    public Operation(T target)
    {
        this.Target = target.Clone();
    }
    public T Target { get; set; }
    public event Action<bool,T> OnResult;
    public void Finish(bool successful)
    {
        base.Finish(successful);
        OnResult.Invoke(successful,Target);
    }
}

public class Operation
{
    public event Action<bool> OnFinished;

    public void Finish(bool successful)
    {
        OnFinished?.Invoke(successful);
    }
}
