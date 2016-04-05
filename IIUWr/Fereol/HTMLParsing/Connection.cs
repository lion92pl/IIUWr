using IIUWr.Fereol.HTMLParsing.Interface;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace IIUWr.Fereol.HTMLParsing
{
    public class Connection : IConnection, IDisposable
    {
        private static readonly Uri Endpoint = new Uri(@"https://zapisy.ii.uni.wroc.pl/");

        public Uri FereolBaseUri { get; set; }

        private HttpBaseProtocolFilter httpFilter;
        private HttpClient httpClient;

        public Connection()
        {
            httpFilter = new HttpBaseProtocolFilter();
            // Fereol certificate is not updated frequently, so ...
            httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);

            httpClient = new HttpClient(httpFilter);
        }

        public async Task<string> GetStringAsync(string relativeUri)
        {
            return await httpClient.GetStringAsync(new Uri(Endpoint, relativeUri)).AsTask(new HttpProgressHandler(relativeUri));
        }

        private class HttpProgressHandler : IProgress<HttpProgress>
        {
            private string _relativeUri;

            public HttpProgressHandler(string relativeUri)
            {
                _relativeUri = relativeUri;
            }

            public void Report(HttpProgress progress)
            {
                System.Diagnostics.Debug.WriteLine($"Download progress: {progress.BytesReceived}/{progress.TotalBytesToReceive} of {_relativeUri}");
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
            httpFilter.Dispose();
        }

        private void ba(IAsyncOperationWithProgress<string, HttpProgress> op, HttpProgress progress)
        {
            //TODO log to special file
            System.Diagnostics.Debug.WriteLine($"Downloaded {progress.BytesReceived}");
        }

        private void bac(IAsyncOperationWithProgress<string, HttpProgress> op, AsyncStatus progress)
        {
            //TODO log to special file
            System.Diagnostics.Debug.WriteLine($"Downloaded {progress.ToString()}");
        }

        public async Task<bool> CheckConnection()
        {
            var response = await httpClient.GetAsync(Endpoint);
            return response.IsSuccessStatusCode;
        }
    }
}
