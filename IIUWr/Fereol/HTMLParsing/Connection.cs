using IIUWr.Fereol.HTMLParsing.Interface;
using IIUWr.Fereol.HTMLParsing.Utils;
using IIUWr.Fereol.Interface;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace IIUWr.Fereol.HTMLParsing
{
    public class Connection : IHTTPConnection, IDisposable
    {
        private Uri Endpoint { get; }

        public Uri FereolBaseUri { get; set; }

        private HttpBaseProtocolFilter httpFilter;
        private HttpClient httpClient;

        public Connection(Uri uri, ICredentialsManager credentialsManager)
        {
            Endpoint = uri;

            httpFilter = new HttpBaseProtocolFilter();
            // Fereol certificate is not updated frequently, so ...
            httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);

            httpClient = new HttpClient(httpFilter);
        }

        public async Task<string> GetStringAsync(string relativeUri)
        {
            try
            {
                return await httpClient.GetStringAsync(new Uri(Endpoint, relativeUri)).AsTask(new HttpProgressHandler(relativeUri));
            }
            catch
            {
                //TODO handle errors properly
                return string.Empty;
            }
        }
        
        public void Dispose()
        {
            httpClient.Dispose();
            httpFilter.Dispose();
        }
        
        public async Task<bool> CheckConnection()
        {
            var response = await httpClient.GetAsync(Endpoint);
            return response.IsSuccessStatusCode;
        }

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
