using IIUWr.Fereol.HTMLParsing.Interface;
using IIUWr.Fereol.HTMLParsing.Utils;
using IIUWr.Fereol.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace IIUWr.Fereol.HTMLParsing
{
    public class Connection : IHTTPConnection, IDisposable
    {
        private const string LoginPath = @"users/login/";
        private const string SecurityCookieName = "csrftoken";

        private readonly Uri _endpoint;
        
        private readonly HttpBaseProtocolFilter _httpFilter;
        private readonly HttpClient _httpClient;

        public Connection(Uri uri, ICredentialsManager credentialsManager)
        {
            _endpoint = uri;

            _httpFilter = new HttpBaseProtocolFilter();
            // Fereol certificate is not updated frequently, so ...
            httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);

            _httpClient = new HttpClient(_httpFilter);
        }

        public async Task<string> GetStringAsync(string relativeUri)
        {
            try
            {
                return await _httpClient.GetStringAsync(new Uri(_endpoint, relativeUri)).AsTask(new HttpProgressHandler(relativeUri));
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
            var cookie = GetSecurityCookie();
            if (cookie == null)
            {
                return false;
            }
            var formData = new Dictionary<string, string>
            {
                ["id_login"] = username,
                ["id_password"] = password,
                ["csrfmiddlewaretoken"] = cookie.Value
            };
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_endpoint, LoginPath));
            request.Content = new HttpFormUrlEncodedContent(formData);
            //request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue(c[0].Name, c[0].Value));
            var response = await _httpClient.SendRequestAsync(request);
            return false;
        }

        private HttpCookie GetSecurityCookie()
        {
            var cookies = _httpFilter.CookieManager.GetCookies(_endpoint);
            return cookies.FirstOrDefault(c => c.Name == SecurityCookieName);
        }
    }
}
