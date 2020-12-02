using System;
using System.ComponentModel;

namespace SampleCode._2_Events
{
    public sealed class DownloadCompletedEventArgs : AsyncCompletedEventArgs
    {
        public DownloadCompletedEventArgs(Exception error) : this(error, null) { }
        public DownloadCompletedEventArgs(string data) : this(null, data) { }

        public string Data
        {
            get
            {
                RaiseExceptionIfNecessary();
                return _data;
            }
        }

        private DownloadCompletedEventArgs(Exception error, string data)
            : base(error, false, null)
        {
            _data = data;
        }
        
        private readonly string _data;
    }

    public delegate void DownloadCompletedDelegate(object sender, DownloadCompletedEventArgs args);

    public interface IDownloadFromInternet
    {
        void DownloadAsync();
        event DownloadCompletedDelegate DownloadCompleted;
    }
}
