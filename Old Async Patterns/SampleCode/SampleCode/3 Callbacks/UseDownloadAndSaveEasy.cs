using System;

namespace SampleCode._3_Callbacks
{
    public interface IUseDownloadAndSave
    {
        void DownloadAndSave(Action<Exception> callback);
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

        public void DownloadAndSave(Action<Exception> callback)
        {
            _downloadFromInternet.DownloadAsync((ex, data) =>
            {
                if (ex != null)
                {
                    callback(ex);
                    return;
                }

                _saveToDatabase.SaveAsync(data, callback);
            });
        }

        //public void DownloadAndSave()
        //{
        //    var data = _downloadFromInternet.Download();
        //    _saveToDatabase.Save(data);
        //}
    }
}
