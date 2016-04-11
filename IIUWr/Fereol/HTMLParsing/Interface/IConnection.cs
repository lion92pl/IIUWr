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
    }
}
