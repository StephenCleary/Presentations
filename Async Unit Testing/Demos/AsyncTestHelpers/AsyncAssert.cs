using System;
using System.Threading.Tasks;

public static class AsyncAssert
{
    /// <summary>
    /// Ensures that an asynchronous delegate throws an exception of an expected type.
    /// </summary>
    /// <typeparam name="TException">The type of exception to expect.</typeparam>
    /// <param name="action">The asynchronous delegate to test.</param>
    /// <param name="allowDerivedTypes">Whether derived types should be accepted.</param>
    public static async Task<TException> ThrowsAsync<TException>(Func<Task> action, bool allowDerivedTypes = true)
        where TException : Exception
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (allowDerivedTypes && !(ex is TException))
                throw new Exception("Delegate threw exception of type " + ex.GetType().Name + ", but " + typeof (TException).Name + " or a derived type was expected.", ex);
            if (!allowDerivedTypes && ex.GetType() != typeof (TException))
                throw new Exception("Delegate threw exception of type " + ex.GetType().Name + ", but " + typeof (TException).Name + " was expected.", ex);
            return (TException)ex;
        }
        throw new Exception("Delegate did not throw expected exception " + typeof (TException).Name + ".");
    }
}
