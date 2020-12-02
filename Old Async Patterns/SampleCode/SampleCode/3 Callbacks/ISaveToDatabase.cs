using System;

namespace SampleCode._3_Callbacks
{
    public interface ISaveToDatabase
    {
        void SaveAsync(string data, Action<Exception> callback);
    }
}
