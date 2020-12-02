using System;

namespace SampleCode._3_Callbacks
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

        public void DownloadAndSave(Action<Exception> callback)
        {
            var state = new State(callback);
            _downloadFromInternet1.DownloadAsync((ex, data1) => Download1Callback(ex, data1, state));
            _downloadFromInternet2.DownloadAsync((ex, data2) => Download2Callback(ex, data2, state));
        }

        private void Download1Callback(Exception ex, string data1, State state)
        {
            if (ex != null)
            {
                lock (state)
                {
                    if (state.IsCompleted)
                        return;
                    state.IsCompleted = true;
                }

                state.Callback(ex);
                return;
            }

            bool readyToWrite;
            lock (state)
            {
                state.Data1 = data1;
                state.HasData1 = true;
                readyToWrite = state.HasData1 && state.HasData2;
            }

            if (readyToWrite)
                SaveResults(state);
        }

        private void Download2Callback(Exception ex, string data2, State state)
        {
            if (ex != null)
            {
                lock (state)
                {
                    if (state.IsCompleted)
                        return;
                    state.IsCompleted = true;
                }

                state.Callback(ex);
                return;
            }

            bool readyToWrite;
            lock (state)
            {
                state.Data2 = data2;
                state.HasData2 = true;
                readyToWrite = state.HasData1 && state.HasData2;
            }

            if (readyToWrite)
                SaveResults(state);
        }

        private void SaveResults(State state)
        {
            _saveToDatabase.SaveAsync(state.Data1 + state.Data2, state.Callback);
        }

        private sealed class State
        {
            public State(Action<Exception> callback) => Callback = callback;

            // Results
            public string Data1 { get; set; }
            public string Data2 { get; set; }

            // State
            public bool HasData1 { get; set; }
            public bool HasData2 { get; set; }
            public bool IsCompleted { get; set; }

            // Completion
            public Action<Exception> Callback { get; }
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
