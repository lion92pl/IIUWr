using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;

namespace IIUWr.Fereol.HTMLParsing.Interface
{
    public interface IHTTPConnection : Fereol.Interface.IConnection
    {
        Task<string> GetStringAsync(string relativeUri);

        Task<string> GetStringFromAPIAsync(string relativeUri);

        Task<string> Post(string relativeUri, Dictionary<string, string> formData, bool addMiddlewareToken = true);
    }
}
