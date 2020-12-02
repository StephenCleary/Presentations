using System.Threading.Tasks;

namespace SampleCode._4_Futures
{
    public interface ISaveToDatabase
    {
        Task SaveAsync(string data);
    }
}
