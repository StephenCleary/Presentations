using System.Threading.Tasks;

namespace SampleCode._5_AsyncAwait
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

        public async Task DownloadAndSaveAsync()
        {
            var task1 = _downloadFromInternet1.DownloadAsync();
            var task2 = _downloadFromInternet2.DownloadAsync();
            var data = await Task.WhenAll(task1, task2);
            await _saveToDatabase.SaveAsync(data[0] + data[1]);
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
