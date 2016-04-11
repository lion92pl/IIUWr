using System;
using System.Diagnostics;
using Windows.Web.Http;

namespace IIUWr.Fereol.HTMLParsing.Utils
{
    public class HttpProgressHandler : IProgress<HttpProgress>
    {
        private string _relativeUri;

        public HttpProgressHandler(string relativeUri)
        {
            _relativeUri = relativeUri;
        }

        public void Report(HttpProgress progress)
        {
            Debug.WriteLine($"Download progress: {progress.BytesReceived}/{progress.TotalBytesToReceive} of {_relativeUri}");
        }
    }
}
