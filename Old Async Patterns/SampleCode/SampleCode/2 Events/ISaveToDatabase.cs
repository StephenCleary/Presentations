using System.ComponentModel;

namespace SampleCode._2_Events
{
    public interface ISaveToDatabase
    {
        void SaveAsync(string data);
        event AsyncCompletedEventHandler SaveCompleted;
    }
}
