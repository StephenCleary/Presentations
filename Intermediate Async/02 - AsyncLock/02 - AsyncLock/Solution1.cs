using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Original idea from Stephen Toub: http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx

// Desired API as it would look in synchronous code:
//   public IDisposable Lock();
// Asychronous equivalent:
//   public Task<IDisposable> LockAsync();
// The IDisposable result is a "key" for the lock that you Dispose to release the lock.

/// <summary>
/// A mutual exclusion lock that is compatible with async. Note that this lock is <b>not</b> recursive!
/// </summary>
public sealed class AsyncLock
{
    /// <summary>
    /// Whether the lock is taken by a task.
    /// </summary>
    private bool taken;

    /// <summary>
    /// The queue of TCSs that other tasks are awaiting to acquire the lock.
    /// </summary>
    private readonly Queue<TaskCompletionSource<IDisposable>> queue = new Queue<TaskCompletionSource<IDisposable>>();

    /// <summary>
    /// Asynchronously acquires the lock. Returns a disposable that releases the lock when disposed.
    /// </summary>
    /// <returns>A disposable that releases the lock when disposed.</returns>
    public Task<IDisposable> LockAsync()
    {
        lock (queue)
        {
            // If the lock is available, take it immediately and return.
            if (!taken)
            {
                taken = true;
                return Task.FromResult<IDisposable>(new Key(this));
            }

            /*
             * If we're here, then the lock is already taken;
             * there is a key already out there that will eventually be disposed.
             */

            // Wait for the lock to become available.
            var tcs = new TaskCompletionSource<IDisposable>();
            queue.Enqueue(tcs);
            return tcs.Task;
        }
    }

    /// <summary>
    /// Releases the lock.
    /// </summary>
    internal void ReleaseLock()
    {
        /*
         * This method is called by the key when it is disposed.
         */

        TaskCompletionSource<IDisposable> tcs = null;
        lock (queue)
        {
            if (queue.Count == 0)
            {
                // There are no tasks waiting for this lock, so mark it as "free" (not taken).
                taken = false;
                return;
            }

            tcs = queue.Dequeue();
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