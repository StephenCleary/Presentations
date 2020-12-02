using System.Threading.Tasks;

namespace SampleCode._5_AsyncAwait
{
    public interface ISaveToDatabase
    {
        Task SaveAsync(string data);
    }
}
