using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCode._3B_IAsyncResult
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

        public IAsyncResult BeginDownloadAndSave(AsyncCallback callback, object userState)
        {
            var localState = new State(userState);
            _downloadFromInternet1.BeginDownload(Download1Callback, localState);
            _downloadFromInternet2.BeginDownload(Download2Callback, localState);
            return localState.TaskCompletionSource.Task;
        }

        private void Download1Callback(IAsyncResult ar)
        {
            var localState = (State)ar.AsyncState;
            string data1;
            try
            {
                data1 = _downloadFromInternet1.EndDownload(ar);
            }
            catch (Exception exception)
            {
                localState!.TaskCompletionSource.TrySetException(exception);
                return;
            }

            bool readyToWrite;
            lock (localState)
            {
                localState.Data1 = data1;
                localState.HasData1 = true;
                readyToWrite = localState.HasData1 && localState.HasData2;
            }

            if (readyToWrite)
                SaveResults(localState);
        }

        private void Download2Callback(IAsyncResult ar)
        {
            var localState = (State)ar.AsyncState;
            string data2;
            try
            {
                data2 = _downloadFromInternet2.EndDownload(ar);
            }
            catch (Exception exception)
            {
                localState!.TaskCompletionSource.TrySetException(exception);
                return;
            }

            bool readyToWrite;
            lock (localState)
            {
                localState.Data2 = data2;
                localState.HasData2 = true;
                readyToWrite = localState.HasData1 && localState.HasData2;
            }

            if (readyToWrite)
                SaveResults(localState);
        }

        private void SaveResults(State state)
        {
            _saveToDatabase.BeginSave(state.Data1 + state.Data2, SaveCallback, state);
        }

        private void SaveCallback(IAsyncResult ar)
        {
            var localState = (State)ar.AsyncState;
            try
            {
                _saveToDatabase.EndSave(ar);
            }
            catch (Exception exception)
            {
                localState!.TaskCompletionSource.TrySetException(exception);
                return;
            }

            localState!.TaskCompletionSource.TrySetResult(null);
        }

        public void EndDownloadAndSave(IAsyncResult result)
        {
            var task = (Task<object>)result;
            if (task.IsFaulted)
                ExceptionDispatchInfo.Capture(task.Exception!.InnerException!).Throw();
        }

        private sealed class State
        {
            public State(object userState) => TaskCompletionSource = new TaskCompletionSource<object>(userState);

            // Results
            public string Data1 { get; set; }
            public string Data2 { get; set; }

            // State
            public bool HasData1 { get; set; }
            public bool HasData2 { get; set; }

            // Completion
            public TaskCompletionSource<object> TaskCompletionSource { get; }
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
