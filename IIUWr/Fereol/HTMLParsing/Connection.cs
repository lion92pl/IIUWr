using IIUWr.Fereol.HTMLParsing.Interface;
using IIUWr.Fereol.HTMLParsing.Utils;
using IIUWr.Fereol.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace IIUWr.Fereol.HTMLParsing
{
    public class Connection : IHTTPConnection, IDisposable
    {
        private const string LoginPath = @"users/login/";
        private const string SecurityCookieName = "csrftoken";
        private const string SessionCookieName = "sessionid";

        private readonly Uri _endpoint;
        
        private readonly HttpBaseProtocolFilter _httpFilter;
        private readonly HttpClient _httpClient;

        private readonly HttpBaseProtocolFilter _httpFilterForLogin;
        private readonly HttpClient _httpClientForLogin;

        private readonly ICredentialsManager _credentialsManager;
        private readonly ISessionManager _sessionManager;

        public Connection(Uri uri, ICredentialsManager credentialsManager, ISessionManager sessionManager)
        {
            _endpoint = uri;
            _credentialsManager = credentialsManager;
            _sessionManager = sessionManager;
            
            _httpFilter = new HttpBaseProtocolFilter();
            _httpFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);

            _httpFilterForLogin = new HttpBaseProtocolFilter
            {
                AllowAutoRedirect = false
            };
            _httpFilterForLogin.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);

            _httpClient = new HttpClient(_httpFilter);
            _httpClientForLogin = new HttpClient(_httpFilterForLogin);

            if (_sessionManager.SessionIdentifier != null)
            {
                _httpFilter.CookieManager.SetCookie(new HttpCookie(SessionCookieName, _endpoint.Host, "/") { Value = _sessionManager.SessionIdentifier } );
            }
            if (_sessionManager.MiddlewareToken != null)
            {
                _httpFilter.CookieManager.SetCookie(new HttpCookie(SecurityCookieName, _endpoint.Host, "/") { Value = _sessionManager.MiddlewareToken });
            }
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
        
        public async Task<bool> CheckConnectionAsync()
        {
            var response = await _httpClient.GetAsync(_endpoint);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var cookie = GetSecurityCookie();
            if (cookie == null)
            {
                return false;
            }
            var formData = new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password,
                ["csrfmiddlewaretoken"] = cookie.Value
            };
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_endpoint, LoginPath))
            {
                Content = new HttpFormUrlEncodedContent(formData)
            };

            _httpFilterForLogin.CookieManager.SetCookie(cookie);

            HttpResponseMessage response = null;
            try
            {
                response = await _httpClientForLogin.SendRequestAsync(request);

                foreach (HttpCookie loginCookie in _httpFilterForLogin.CookieManager.GetCookies(_endpoint))
                {
                    switch (loginCookie.Name)
                    {
                        case SecurityCookieName:
                            _sessionManager.MiddlewareToken = loginCookie.Value;
                            break;
                        case SessionCookieName:
                            _sessionManager.SessionIdentifier = loginCookie.Value;
                            break;
                    }
                    _httpFilter.CookieManager.SetCookie(loginCookie);
                }
                response = await _httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(_endpoint, LoginPath)));
            }
            catch
            {
                return false;
            }

            var page = await response.Content.ReadAsStringAsync();
            var authStatus = CommonRegexes.ParseAuthenticationStatus(page);

            return authStatus?.Authenticated ?? false;
        }

        private HttpCookie GetSecurityCookie()
        {
            var cookies = _httpFilter.CookieManager.GetCookies(_endpoint);
            return cookies.FirstOrDefault(c => c.Name == SecurityCookieName);
        }
    }
}
