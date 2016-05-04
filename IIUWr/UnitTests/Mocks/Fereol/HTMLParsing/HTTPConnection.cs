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
        
        public Task<bool> CheckConnection()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetStringAsync(string relativeUri)
        {
            return await Task.FromResult(_result);
        }

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
