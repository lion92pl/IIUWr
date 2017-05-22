using IIUWr.Fereol.HTMLParsing.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Mocks.Fereol.HTMLParsing
{
    internal class HTTPConnection : IHTTPConnection
    {
        private readonly string _result;
        public HTTPConnection(string htmlFilePath)
        {
            _result = System.IO.File.ReadAllText(htmlFilePath);
        }
        
        public Task<bool> CheckConnectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStringAsync(string relativeUri)
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
    }
}
