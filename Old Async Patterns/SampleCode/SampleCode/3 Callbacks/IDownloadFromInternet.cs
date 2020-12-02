using System;

namespace SampleCode._3_Callbacks
{
    public interface IDownloadFromInternet
    {
        void DownloadAsync(Action<Exception, string> callback);
    }
}
