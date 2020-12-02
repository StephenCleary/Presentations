using System;
using System.Runtime.ExceptionServices;
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

        public IAsyncResult BeginDownloadAndSave(AsyncCallback callback, object state)
        {
            var localState = new State
            {
                TaskCompletionSource = new TaskCompletionSource<object>(state),
            };

            _downloadFromInternet1.BeginDownload(ar =>
            {
                var localState = (State) ar.AsyncState;
                try
                {
                    localState!.Data1 = _downloadFromInternet1.EndDownload(ar);
                }
                catch (Exception exception)
                {
                    localState!.TaskCompletionSource.TrySetException(exception);
                    return;
                }

                Continue(localState);
            }, localState);

            _downloadFromInternet2.BeginDownload(ar =>
            {
                var localState = (State)ar.AsyncState;
                try
                {
                    localState!.Data2 = _downloadFromInternet2.EndDownload(ar);
                }
                catch (Exception exception)
                {
                    localState!.TaskCompletionSource.TrySetException(exception);
                    return;
                }

                Continue(localState);
            }, localState);

            return localState.TaskCompletionSource.Task;
        }

        private void Continue(State state)
        {
            if (!state.HasData1 || !state.HasData2)
                return;

            _saveToDatabase.BeginSave(state.Data1 + state.Data2, ar =>
            {
                var localState = (State) ar.AsyncState;
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
            }, state);
        }

        public void EndDownloadAndSave(IAsyncResult result)
        {
            var task = (Task<object>)result;
            if (task.IsFaulted)
                ExceptionDispatchInfo.Capture(task.Exception!.InnerException!).Throw();
        }

        private sealed class State
        {
            // Results
            public string Data1 { get; set; }
            public string Data2 { get; set; }

            // State
            public bool HasData1 { get; set; }
            public bool HasData2 { get; set; }

            public TaskCompletionSource<object> TaskCompletionSource { get; set; }
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
