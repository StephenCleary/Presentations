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
            _downloadFromInternet1.DownloadAsync().ContinueWith(t => Download1Continuation(t, state));
            _downloadFromInternet2.DownloadAsync().ContinueWith(t => Download2Continuation(t, state));
            return state.TaskCompletionSource.Task;
        }

        private void Download1Continuation(Task<string> t, State state)
        {
            if (t.Exception != null)
            {
                state.TaskCompletionSource.TrySetException(t.Exception.InnerExceptions);
                return;
            }

            bool readyToWrite;
            lock (state)
            {
                state.Data1 = t.Result;
                state.HasData1 = true;
                readyToWrite = state.HasData1 && state.HasData2;
            }

            if (readyToWrite)
                SaveResults(state);
        }

        private void Download2Continuation(Task<string> t, State state)
        {
            if (t.Exception != null)
            {
                state.TaskCompletionSource.TrySetException(t.Exception.InnerExceptions);
                return;
            }

            bool readyToWrite;
            lock (state)
            {
                state.Data2 = t.Result;
                state.HasData2 = true;
                readyToWrite = state.HasData1 && state.HasData2;
            }

            if (readyToWrite)
                SaveResults(state);
        }

        private void SaveResults(State state)
        {
            _saveToDatabase.SaveAsync(state.Data1 + state.Data2).ContinueWith(t => SaveContinuation(t, state));
        }

        private void SaveContinuation(Task t, State state)
        {
            if (t.Exception != null)
                state.TaskCompletionSource.TrySetException(t.Exception.InnerExceptions);
            else
                state.TaskCompletionSource.TrySetResult(null);
        }

        private sealed class State
        {
            // Results
            public string Data1 { get; set; }
            public string Data2 { get; set; }

            // State
            public bool HasData1 { get; set; }
            public bool HasData2 { get; set; }

            // Completion
            public TaskCompletionSource<object> TaskCompletionSource { get; } = new TaskCompletionSource<object>();
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
