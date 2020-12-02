using System;

namespace SampleCode._3B_IAsyncResult
{
    public interface ISaveToDatabase
    {
        IAsyncResult BeginSave(string data, AsyncCallback callback, object state);
        void EndSave(IAsyncResult result);
    }
}
