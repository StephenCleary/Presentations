using System.Threading.Tasks;

namespace SampleCode._1_Synchronous
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

        public void DownloadAndSave()
        {
            string data1 = null;
            string data2 = null;
            Parallel.Invoke(
                () => data1 = _downloadFromInternet1.Download(),
                () => data2 = _downloadFromInternet2.Download());
            _saveToDatabase.Save(data1 + data2);
        }
    }
}
