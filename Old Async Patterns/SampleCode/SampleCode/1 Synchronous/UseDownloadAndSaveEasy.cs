namespace SampleCode._1_Synchronous
{
    public interface IUseDownloadAndSave
    {
        void DownloadAndSave();
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

        public void DownloadAndSave()
        {
            var data = _downloadFromInternet.Download();
            _saveToDatabase.Save(data);
        }
    }
}
