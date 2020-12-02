using System;

namespace SampleCode._3B_IAsyncResult
{
    // https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/asynchronous-programming-model-apm

    public interface IDownloadFromInternet
    {
        IAsyncResult BeginDownload(AsyncCallback callback, object state);
        string EndDownload(IAsyncResult result);
    }
}
