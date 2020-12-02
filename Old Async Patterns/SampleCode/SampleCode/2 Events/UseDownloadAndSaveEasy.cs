using System.ComponentModel;

namespace SampleCode._2_Events
{
    public interface IUseDownloadAndSave
    {
        void DownloadAndSaveAsync();
        event AsyncCompletedEventHandler DownloadAndSaveCompleted;
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

        public event AsyncCompletedEventHandler DownloadAndSaveCompleted;

        public void DownloadAndSaveAsync()
        {
            _downloadFromInternet.DownloadCompleted += (_, args) =>
            {
                if (args.Error != null)
                {
                    DownloadAndSaveCompleted?.Invoke(this, new AsyncCompletedEventArgs(args.Error, false, null));
                    return;
                }

                var data = args.Data;
                _saveToDatabase.SaveCompleted += (_, args) =>
                {
                    DownloadAndSaveCompleted?.Invoke(this, new AsyncCompletedEventArgs(args.Error, false, null));
                };
                _saveToDatabase.SaveAsync(data);
            };
            _downloadFromInternet.DownloadAsync();
        }

        //public void DownloadAndSave()
        //{
        //    var data = _downloadFromInternet.Download();
        //    _saveToDatabase.Save(data);
        //}
    }
}
