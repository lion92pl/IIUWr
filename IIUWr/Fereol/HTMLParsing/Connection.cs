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
using IIUWr.Fereol.Model;
using System.ComponentModel;

namespace IIUWr.Fereol.HTMLParsing
{
    public class Connection : IHTTPConnection, IDisposable
    {
        private const string LoginPath = @"users/login/";
        private const string LogoutPath = @"/users/logout/";
        private const string SecurityTokenFormDataName = "csrfmiddlewaretoken";
        private const string SecurityCookieName = "csrftoken";
        private const string SessionCookieName = "sessionid";
        private readonly Uri _endpoint;
        
        private readonly HttpBaseProtocolFilter _httpFilter;
        private readonly HttpClient _httpClient;

        private readonly HttpBaseProtocolFilter _httpFilterForLogin;
        private readonly HttpClient _httpClientForLogin;

        private readonly ICredentialsManager _credentialsManager;
        private readonly ISessionManager _sessionManager;

        public event EventHandler AuthStatusChanged;

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

        private AuthenticationStatus _authStatus;
        public AuthenticationStatus AuthStatus
        {
            get => _authStatus;
            set
            {
                if (_authStatus != value)
                {
                    _authStatus = value;
                    AuthStatusChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public async Task<string> GetStringAsync(string relativeUri)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(new Uri(_endpoint, relativeUri)).AsTask(new HttpProgressHandler(relativeUri));

                AuthStatus = CommonRegexes.ParseAuthenticationStatus(response);

                return response;
            }
            catch
            {
                //TODO handle errors properly
                return string.Empty;
            }
        }

        public async Task<string> Post(string relativeUri, Dictionary<string, string> formData, bool addMiddlewareToken = true)
        {
            if (formData == null)
            {
                throw new ArgumentNullException(nameof(formData));
            }

            if (addMiddlewareToken)
            {
                var cookie = GetSecurityCookie();
                if (cookie != null)
                {
                    formData.Add(SecurityTokenFormDataName, cookie.Value);
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_endpoint, relativeUri))
            {
                Content = new HttpFormUrlEncodedContent(formData)
            };

            try
            {
                var responseMessage = await _httpClient.SendRequestAsync(request).AsTask(new HttpProgressHandler(relativeUri));
                var response = await responseMessage.Content.ReadAsStringAsync();

                AuthStatus = CommonRegexes.ParseAuthenticationStatus(response);

                return response;
            }
            catch
            {
                //TODO handle errors properly
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _httpFilter.Dispose();

            _httpClientForLogin.Dispose();
            _httpFilterForLogin.Dispose();
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
                await _httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(_endpoint, LoginPath)));
                cookie = GetSecurityCookie();
                if (cookie == null)
                {
                    return false;
                }
            }
            var formData = new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password,
                [SecurityTokenFormDataName] = cookie.Value
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
                SaveCookiesAfterLogin();
                //response = await _httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(_endpoint, LoginPath)));
                response = await _httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, _endpoint));
            }
            catch
            {
                return false;
            }

            var page = await response.Content.ReadAsStringAsync();
            var authStatus = CommonRegexes.ParseAuthenticationStatus(page);

            AuthStatus = authStatus;

            return authStatus?.Authenticated ?? false;
        }

        public async Task<bool> LogoutAsync()
        {
            var cookie = GetSecurityCookie();
            if (cookie == null)
            {
                await _httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(_endpoint, LoginPath)));
                cookie = GetSecurityCookie();
                if (cookie == null)
                {
                    return false;
                }
            }
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_endpoint, LogoutPath));

            _httpFilterForLogin.CookieManager.SetCookie(cookie);

            HttpResponseMessage response = null;
            try
            {
                response = await _httpClientForLogin.SendRequestAsync(request);
                response = await _httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(_endpoint, LoginPath)));
            }
            catch
            {
                return false;
            }

            var page = await response.Content.ReadAsStringAsync();
            var authStatus = CommonRegexes.ParseAuthenticationStatus(page);

            AuthStatus = authStatus;

            return !authStatus?.Authenticated ?? true;
        }

        private void SaveCookiesAfterLogin()
        {
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
        }

        private HttpCookie GetSecurityCookie()
        {
            var cookies = _httpFilter.CookieManager.GetCookies(_endpoint);
            return cookies.FirstOrDefault(c => c.Name == SecurityCookieName);
        }
    }
}
