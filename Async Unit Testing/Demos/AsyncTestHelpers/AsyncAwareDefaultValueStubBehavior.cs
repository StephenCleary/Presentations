using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes.Stubs;

/// <summary>
/// A stub behavior that never returns null tasks.
/// </summary>
[DebuggerNonUserCode]
[Serializable]
public sealed class AsyncAwareDefaultValueStubBehavior : IStubBehavior
{
    /// <summary>
    /// A helper class for task constants.
    /// </summary>
    /// <typeparam name="TResult">The type of the task results.</typeparam>
    public static class TaskConstants<TResult>
    {
        /// <summary>
        /// A task completed with the async-aware default value of <typeparamref name="TResult"/>.
        /// </summary>
        public static readonly Task<TResult> Default = Task.FromResult(GetDefaultValue<TResult>());
    }

    /// <summary>
    /// Gets the async-aware default value of the specified type. If the type is <see cref="Task"/> or <see cref="Task{T}"/>, then a completed task of the appropriate type is returned.
    /// </summary>
    /// <typeparam name="TResult">The type of default value to get.</typeparam>
    public static TResult GetDefaultValue<TResult>()
    {
        var resultType = typeof (TResult);
        if (resultType == typeof (Task))
            return (TResult)(object)TaskConstants<object>.Default;
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof (Task<>))
            return (TResult)typeof (TaskConstants<>).MakeGenericType(resultType.GenericTypeArguments)
                .GetField("Default", BindingFlags.Static | BindingFlags.Public).GetValue(null);
        return default(TResult);
    }

    TResult IStubBehavior.Result<TStub, TResult>(TStub target, string name)
    {
        return GetDefaultValue<TResult>();
    }

    bool IStubBehavior.TryGetValue<TValue>(object name, out TValue value)
    {
        value = default(TValue);
        return true;
    }

    void IStubBehavior.ValueAtReturn<TStub, TValue>(TStub target, string name, out TValue value)
    {
        value = default(TValue);
    }

    void IStubBehavior.ValueAtEnterAndReturn<TStub, TValue>(TStub target, string name, ref TValue value)
    {
        value = default(TValue);
    }

    void IStubBehavior.VoidResult<TStub>(TStub target, string name)
    {
    }
}
