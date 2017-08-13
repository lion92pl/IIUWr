using IIUWr.Fereol.HTMLParsing.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIUWr.Fereol.Model;

namespace UnitTests.Mocks.Fereol.HTMLParsing
{
    internal class HTTPConnection : IHTTPConnection
    {
        private readonly string _result;

        public event EventHandler AuthStatusChanged;

        public AuthenticationStatus AuthStatus { get; private set; }

        public HTTPConnection(string htmlFilePath) : this(htmlFilePath, null) { }

        public HTTPConnection(string htmlFilePath, AuthenticationStatus authStatus)
        {
            _result = System.IO.File.ReadAllText(htmlFilePath);
            AuthStatus = authStatus;
        }
        
        public Task<bool> CheckConnectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStringAsync(string relativeUri)
        {
            return Task.FromResult(_result);
        }

        public Task<string> GetStringFromAPIAsync(string relativeUri)
        {
            return Task.FromResult(_result);
        }

        public Task<string> Post(string relativeUri, Dictionary<string, string> formData, bool addMiddlewareToken = true)
        {
            return Task.FromResult(_result);
        }

        public Task<bool> LoginAsync(string username, string password)
        {
            return Task.FromResult(username == "test" && password == "mock");
        }

        public Task<bool> LogoutAsync()
        {
            return Task.FromResult(true);
        }

        internal void ChangeAuthStatus(AuthenticationStatus authStatus)
        {
            AuthStatus = authStatus;
            AuthStatusChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
