using System.Threading.Tasks;

namespace SampleCode._4_Futures
{
    public interface IDownloadFromInternet
    {
        Task<string> DownloadAsync();
    }
}
