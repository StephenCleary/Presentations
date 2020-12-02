using System.Threading.Tasks;

namespace SampleCode._5_AsyncAwait
{
    public interface IUseDownloadAndSave
    {
        Task DownloadAndSaveAsync();
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

        public async Task DownloadAndSaveAsync()
        {
            var data = await _downloadFromInternet.DownloadAsync();
            await _saveToDatabase.SaveAsync(data);
        }

        //public void DownloadAndSave()
        //{
        //    var data = _downloadFromInternet.Download();
        //    _saveToDatabase.Save(data);
        //}
    }
}
