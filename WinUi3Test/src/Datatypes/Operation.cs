using System;
using WinUi3Test.src.Storage;

namespace WinUi3Test.src.ViewModel;

public class Operation<T> where T : class, Clonable<T>
{
    public Operation(T target)
    {
        this.target = target.Clone();
    }
    public T target { get; private set; }
    public event Action<T> onFinished;
    public void Finish(bool successful)
    {
        T result = successful ? target : null;
        onFinished.Invoke(result);
    }
}