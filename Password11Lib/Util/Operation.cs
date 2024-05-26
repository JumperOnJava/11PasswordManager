using Password11.Datatypes.Serializing;

namespace Password11.Datatypes;

/// <summary>
///     Class for creating operation that can be succesful or unsuccessful
/// </summary>
public class Operation
{
    /// <summary>
    ///     Event that invokes on operation finish
    ///     Resets after finish
    /// </summary>
    public event Action<bool> OnFinished;

    public async Task<bool> GetFinished()
    {
        var tcs = new TaskCompletionSource<bool>();
        OnFinished += tcs.SetResult;
        return await tcs.Task;
    }

    /// <summary>
    ///     Triggers operation finish
    /// </summary>
    /// <param name="successful"></param>
    public virtual void Finish(bool successful)
    {
        OnFinished?.Invoke(successful);
        OnFinished = null;
    }

    public void FinishFail()
    {
        Finish(false);
    }
}

/// <summary>
///     Allows you to manage operations on data with possible failure like user cancel changes
/// </summary>
/// <typeparam name="T">Type on which operation happens </typeparam>
public class Operation<T> : Operation
{
    /// <summary>
    ///     Passes <typeparamref name="T" /> clone as operation target
    ///     Used when you want user to be able to cancel his changes
    /// </summary>
    /// <param name="target"></param>
    public Operation(RefClonable<T> target) : this(target.CloneRef)
    {
    }

    /// <summary>
    ///     Passes object generated by <typeparamref name="T" /> factory as operation target
    ///     Useful to create new objects with some data initialized
    /// </summary>
    public Operation(Func<T> Factory)
    {
        Target = Factory.Invoke();
    }
    public Operation() : this(() => default)
    {
    }

    public T Target { get; set; }

    /// <summary>
    ///     Event that happens on finish, passes success status and target
    ///     Reset after finish
    /// </summary>
    public event Action<bool, T> OnResult;

    public async Task<Tuple<bool, T>> GetResult()
    {
        var tcs = new TaskCompletionSource<Tuple<bool, T>>();
        OnResult += (b, arg2) => tcs.SetResult(new Tuple<bool, T>(b, arg2));
        return await tcs.Task;
    }

    /// <summary>
    ///     Triggers OnFinish and OnResult events with target object as parameter
    /// </summary>
    /// <param name="successful"></param>
    public override void Finish(bool successful)
    {
        base.Finish(successful);
        OnResult?.Invoke(successful, Target);
        OnResult = null;
    }

    public async Task<bool> GetFinish()
    {
        var tcs = new TaskCompletionSource<bool>();
        OnFinished += b => tcs.SetResult(b);
        return await tcs.Task;
    }

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    public void FinishSuccess(T result)
    {
        Target = result;
        Finish(true);
    }
}