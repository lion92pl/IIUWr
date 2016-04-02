using IIUWr.Fereol.HTMLParsing.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public IAsyncOperationWithProgress<string, HttpProgress> GetStringAsync(string relativeUri)
        {
            return httpClient.GetStringAsync(new Uri(Endpoint, relativeUri));
        }

        public void Dispose()
        {
            httpClient.Dispose();
            httpFilter.Dispose();
        }
    }
}
