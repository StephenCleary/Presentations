using System.Threading.Tasks;

// https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming

namespace SampleCode._4_Futures
{
    public interface IUseDownloadAndSave
    {
        Task DownloadAndSaveAsync();
    }

    public sealed class UseDownloadAndSaveEasy1 : IUseDownloadAndSave
    {
        private readonly IDownloadFromInternet _downloadFromInternet;
        private readonly ISaveToDatabase _saveToDatabase;

        public UseDownloadAndSaveEasy1(IDownloadFromInternet downloadFromInternet, ISaveToDatabase saveToDatabase)
        {
            _downloadFromInternet = downloadFromInternet;
            _saveToDatabase = saveToDatabase;
        }

        public Task DownloadAndSaveAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            _downloadFromInternet.DownloadAsync().ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                    return;
                }

                _saveToDatabase.SaveAsync(t.Result).ContinueWith(t =>
                {
                    if (t.Exception != null)
                        tcs.TrySetException(t.Exception.InnerExceptions);
                    else
                        tcs.TrySetResult(null);
                });
            });
            return tcs.Task;
        }

        //public void DownloadAndSave()
        //{
        //    var data = _downloadFromInternet.Download();
        //    _saveToDatabase.Save(data);
        //}
    }
}
