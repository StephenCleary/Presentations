using System;
using System.ComponentModel;

namespace SampleCode._2_Events
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

        public event AsyncCompletedEventHandler DownloadAndSaveCompleted;

        public void DownloadAndSaveAsync()
        {
            var state = new State();
            _downloadFromInternet1.DownloadCompleted += (_, args) => Download1Completed(args, state);
            _downloadFromInternet1.DownloadAsync();

            _downloadFromInternet2.DownloadCompleted += (_, args) => Download2Completed(args, state);
            _downloadFromInternet2.DownloadAsync();
        }

        private void Download1Completed(DownloadCompletedEventArgs args, State state)
        {
            if (args.Error != null)
            {
                lock (state)
                {
                    if (state.IsCompleted)
                        return;
                    state.IsCompleted = true;
                }

                DownloadAndSaveCompleted?.Invoke(this, new AsyncCompletedEventArgs(args.Error, false, null));
                return;
            }

            bool readyToWrite;
            lock (state)
            {
                state.Data1 = args.Data;
                state.HasData1 = true;
                readyToWrite = state.HasData1 && state.HasData2;
            }

            if (readyToWrite)
                SaveResults(state);
        }

        private void Download2Completed(DownloadCompletedEventArgs args, State state)
        {
            if (args.Error != null)
            {
                lock (state)
                {
                    if (state.IsCompleted)
                        return;
                    state.IsCompleted = true;
                }

                DownloadAndSaveCompleted?.Invoke(this, new AsyncCompletedEventArgs(args.Error, false, null));
                return;
            }

            bool readyToWrite;
            lock (state)
            {
                state.Data2 = args.Data;
                state.HasData2 = true;
                readyToWrite = state.HasData1 && state.HasData2;
            }

            if (readyToWrite)
                SaveResults(state);
        }

        private void SaveResults(State state)
        {
            _saveToDatabase.SaveCompleted += (_, args) =>
            {
                DownloadAndSaveCompleted?.Invoke(this, args);
            };

            _saveToDatabase.SaveAsync(state.Data1 + state.Data2);
        }

        private sealed class State
        {
            // Results
            public string Data1 { get; set; }
            public string Data2 { get; set; }

            // State
            public bool HasData1 { get; set; }
            public bool HasData2 { get; set; }
            public bool IsCompleted { get; set; }
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
