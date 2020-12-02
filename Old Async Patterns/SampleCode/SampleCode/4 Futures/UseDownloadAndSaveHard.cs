using System.Threading.Tasks;

namespace SampleCode._4_Futures
{
    public sealed class UseDownloadAndSaveHard : IUseDownloadAndSave
    {
        private readonly IDownloadFromInternet _downloadFromInternet1;
        private readonly IDownloadFromInternet _downloadFromInternet2;
        private readonly ISaveToDatabase _saveToDatabase;

        public UseDownloadAndSaveHard(IDownloadFromInternet downloadFromInternet1, IDownloadFromInternet downloadFromInternet2, ISaveToDatabase saveToDatabase)
        {
            _downloadFromInternet1 = downloadFromInternet1;
            _downloadFromInternet2 = downloadFromInternet2;
            _saveToDatabase = saveToDatabase;
        }

        public Task DownloadAndSaveAsync()
        {
            var state = new State();
            var tcs = new TaskCompletionSource<object>();
            _downloadFromInternet1.DownloadAsync().ContinueWith(t => DownloadContinuation(t, true));
            _downloadFromInternet2.DownloadAsync().ContinueWith(t => DownloadContinuation(t, false));
            return tcs.Task;

            void DownloadContinuation(Task<string> task, bool isForData1)
            {
                if (task.Exception != null)
                {
                    tcs.TrySetException(task.Exception.InnerExceptions);
                    return;
                }

                bool readyToWrite;
                lock (state)
                {
                    if (isForData1)
                    {
                        state.Data1 = task.Result;
                        state.HasData1 = true;
                    }
                    else
                    {
                        state.Data2 = task.Result;
                        state.HasData2 = true;
                    }

                    readyToWrite = state.HasData1 && state.HasData2;
                }

                if (!readyToWrite)
                    return;

                _saveToDatabase.SaveAsync(state.Data1 + state.Data2).ContinueWith(t =>
                {
                    if (t.Exception != null)
                        tcs.TrySetException(t.Exception.InnerExceptions);
                    else
                        tcs.TrySetResult(null);
                });
            }
        }
        
        private sealed class State
        {
            // Results
            public string Data1 { get; set; }
            public string Data2 { get; set; }

            // State
            public bool HasData1 { get; set; }
            public bool HasData2 { get; set; }
        }

        //public void DownloadAndSave()
        //{
        //    string data1 = null;
        //    string data2 = null;
        //    Parallel.Invoke(
        //        () => data1 = _downloadFromInternet1.Download(),
        //        () => data2 = _downloadFromInternet2.Download());
        //    _saveToDatabase.Save(data1 + data2);
        //}
    }
}
