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
        private const string LoginPath = @"users/login/";

        private readonly Uri _endpoint;
        
        private readonly HttpBaseProtocolFilter _httpFilter;
        private readonly HttpClient _httpClient;

        public Connection(Uri uri, ICredentialsManager credentialsManager)
        {
            _endpoint = uri;

            _httpFilter = new HttpBaseProtocolFilter();
            _httpClient = new HttpClient(_httpFilter);

            // Fereol certificate is not updated frequently, so ...
            //TODO uncomment if certificate expires again
            //_httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
        }

        public async Task<string> GetStringAsync(string relativeUri)
        {
            try
            {
                string page;
                //var task = _httpClient.GetAsync(new Uri(_endpoint, relativeUri));
                //task.Progress = (a,b) => System.Diagnostics.Debug.WriteLine($"Download progress: {b.BytesReceived}/{b.TotalBytesToReceive} of {relativeUri}");
                //var result = await task;
                //page = await result.Content.ReadAsStringAsync();

                page = await _httpClient.GetStringAsync(new Uri(_endpoint, relativeUri)).AsTask(new HttpProgressHandler(relativeUri));

                return page;
            }
            catch
            {
                //TODO handle errors properly
                return string.Empty;
            }
        }
        
        public void Dispose()
        {
            _httpClient.Dispose();
            _httpFilter.Dispose();
        }
        
        public async Task<bool> CheckConnection()
        {
            var response = await _httpClient.GetAsync(_endpoint);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Login(string username, string password)
        {
            var formData = new System.Collections.Generic.Dictionary<string, string>
            {
                ["id_login"] = username,
                ["id_password"] = password
            };
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_endpoint, LoginPath));
            request.Content = new HttpFormUrlEncodedContent(formData);
            var response = await _httpClient.SendRequestAsync(request);
            throw new NotImplementedException();
        }
    }
}
