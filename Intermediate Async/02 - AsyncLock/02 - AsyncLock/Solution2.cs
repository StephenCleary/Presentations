using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * CancellationToken support.
 * 
 * AsyncLock mutex; // Can be used as:
 * 
 * using (await mutex.LockAsync(CT.None)) { } // Regular lock()
 * await mutex.LockAsync(new CTS(TimeSpan.FromSeconds(5)).Token); // Timed lock
 * await mutex.LockAsync(myToken); // Cancelable lock
 * await mutex.LockAsync(new CT(true)); // TryLock
 */

/// <summary>
/// A mutual exclusion lock that is compatible with async. Note that this lock is <b>not</b> recursive!
/// </summary>
public sealed class AsyncLockWithCancellation
{
    /// <summary>
    /// Whether the lock is taken by a task.
    /// </summary>
    private bool taken;

    /// <summary>
    /// The queue of TCSs that other tasks are awaiting to acquire the lock.
    /// </summary>
    private readonly List<TaskCompletionSource<IDisposable>> queue = new List<TaskCompletionSource<IDisposable>>();

    /// <summary>
    /// Asynchronously acquires the lock. Returns a disposable that releases the lock when disposed.
    /// </summary>
    /// <returns>A disposable that releases the lock when disposed.</returns>
    public Task<IDisposable> LockAsync(CancellationToken token)
    {
        TaskCompletionSource<IDisposable> tcs = new TaskCompletionSource<IDisposable>();

        lock (queue)
        {
            // If the lock is available, take it immediately and return.
            if (!taken)
            {
                taken = true;
                tcs.SetResult(new Key(this));
                return tcs.Task;
            }

            #region Optional but nice behavior
            // If the CancellationToken is signalled, cancel synchronously before entering the queue.
            if (token.IsCancellationRequested)
            {
                tcs.SetCanceled();
                return tcs.Task;
            }
            #endregion

            // Wait for the lock to become available (or cancellation).
            queue.Add(tcs);
        } // (release lock)

        // We have to run this outside the lock because Register may run its delegate synchronously.
        if (token.CanBeCanceled)
        {
            token.Register(() =>
            {
                lock (queue)
                {
                    if (!queue.Remove(tcs))
                        return;
                }

                // Again, complete the TCS *outside* any locks!
                tcs.SetCanceled();
            }, useSynchronizationContext: false);
        }

        return tcs.Task;
    }

    /// <summary>
    /// Releases the lock.
    /// </summary>
    internal void ReleaseLock()
    {
        TaskCompletionSource<IDisposable> tcs = null;
        lock (queue)
        {
            if (queue.Count == 0)
            {
                // There are no tasks waiting for this lock, so mark it as "free" (not taken).
                taken = false;
                return;
            }

            // This is an O(N) operation. We should really use a double-ended queue instead of a list.
            tcs = queue[0];
            queue.RemoveAt(0);
        }

        // Complete the TCS *outside* the lock!
        // Alternatively, if we had to complete inside the lock, we could do:
        //   Task.Run(() => tcs.SetResult(new Key(this)));
        //   tcs.Task.Wait();
        // You may want to do that anyway, to prevent task continuations running in a finally block.
        tcs.SetResult(new Key(this));
    }

    /// <summary>
    /// The disposable which releases the lock.
    /// </summary>
    private sealed class Key : IDisposable
    {
        /// <summary>
        /// The lock to release.
        /// </summary>
        private AsyncLock asyncLock;

        /// <summary>
        /// Creates the key for a lock.
        /// </summary>
        /// <param name="asyncLock">The lock to release. May not be <c>null</c>.</param>
        public Key(AsyncLock asyncLock)
        {
            this.asyncLock = asyncLock;
        }

        /// <summary>
        /// Release the lock.
        /// </summary>
        public void Dispose()
        {
            if (asyncLock == null)
                return;
            asyncLock.ReleaseLock();
            asyncLock = null;
        }
    }
}