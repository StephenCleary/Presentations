using System;
using System.Threading.Tasks;

namespace SampleCode._4_Futures
{
    public static class TaskThen
    {
        // Simplified version of https://devblogs.microsoft.com/pfxteam/processing-sequences-of-asynchronous-operations-with-tasks/
        public static Task Then<T1>(this Task<T1> first, Func<T1, Task> next)
        {
            var tcs = new TaskCompletionSource<object>();
            first.ContinueWith(_ =>
            {
                if (first.Exception != null)
                    tcs.TrySetException(first.Exception.InnerExceptions);
                else
                {
                    next(first.Result).ContinueWith(t =>
                    {
                        if (t.Exception != null)
                            tcs.TrySetException(t.Exception.InnerExceptions);
                        else
                            tcs.TrySetResult(null);
                    });
                }
            });
            return tcs.Task;
        }
    }

    public sealed class UseDownloadAndSaveEasy2 : IUseDownloadAndSave
    {
        private readonly IDownloadFromInternet _downloadFromInternet;
        private readonly ISaveToDatabase _saveToDatabase;

        public UseDownloadAndSaveEasy2(IDownloadFromInternet downloadFromInternet, ISaveToDatabase saveToDatabase)
        {
            _downloadFromInternet = downloadFromInternet;
            _saveToDatabase = saveToDatabase;
        }

        public Task DownloadAndSaveAsync()
        {
            return _downloadFromInternet.DownloadAsync()
                .Then(data => _saveToDatabase.SaveAsync(data));
        }

        //public void DownloadAndSave()
        //{
        //    var data = _downloadFromInternet.Download();
        //    _saveToDatabase.Save(data);
        //}
    }
}
