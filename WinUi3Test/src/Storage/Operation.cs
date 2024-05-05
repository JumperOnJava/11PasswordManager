using System;
using WinUi3Test.src.Storage;

namespace WinUi3Test.src.ViewModel;

public interface Operation<T>
{
    public T Target { get; }
    public event Action<Account?> onFinished;
    public void Finish(bool successful);
}