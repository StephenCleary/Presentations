using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace SampleCode._3B_IAsyncResult
{
    public interface IUseDownloadAndSave
    {
        IAsyncResult BeginDownloadAndSave(AsyncCallback callback, object state);
        void EndDownloadAndSave(IAsyncResult result);
    }

    public sealed class UseDownloadAndSaveEasy : IUseDownloadAndSave
    {
        private readonly IDownloadFromInternet _downloadFromInternet;
        private readonly ISaveToDatabase _saveToDatabase;

        public UseDownloadAndSaveEasy(IDownloadFromInternet downloadFromInternet, ISaveToDatabase saveToDatabase)
        {
            _downloadFromInternet = downloadFromInternet;
            _saveToDatabase = saveToDatabase;
        }

        // Very old-school implementations use an IAsyncResult by Jeffrey Richter from an old MSDN article:
        //   https://docs.microsoft.com/en-us/archive/msdn-magazine/2007/march/implementing-the-clr-asynchronous-programming-model
        // This example code uses the much simpler Task/TaskCompletionSource approach introduced in .NET 4.0.

        public IAsyncResult BeginDownloadAndSave(AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<object>(state);
            _downloadFromInternet.BeginDownload(ar =>
            {
                string data;
                try
                {
                    data = _downloadFromInternet.EndDownload(ar);
                }
                catch (Exception exception)
                {
                    tcs.TrySetException(exception);
                    return;
                }

                _saveToDatabase.BeginSave(data, ar =>
                {
                    try
                    {
                        _saveToDatabase.EndSave(ar);
                    }
                    catch (Exception exception)
                    {
                        tcs.TrySetException(exception);
                        return;
                    }

                    tcs.TrySetResult(null);
                }, null);
            }, null);
            return tcs.Task;
        }

        public void EndDownloadAndSave(IAsyncResult result)
        {
            var task = (Task<object>) result;
            if (task.IsFaulted)
                ExceptionDispatchInfo.Capture(task.Exception!.InnerException!).Throw();
        }

        //public void DownloadAndSave()
        //{
        //    var data = _downloadFromInternet.Download();
        //    _saveToDatabase.Save(data);
        //}
    }
}
