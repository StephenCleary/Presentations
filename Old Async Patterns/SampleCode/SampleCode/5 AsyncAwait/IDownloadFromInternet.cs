using System.Threading.Tasks;

namespace SampleCode._5_AsyncAwait
{
    public interface IDownloadFromInternet
    {
        Task<string> DownloadAsync();
    }
}
