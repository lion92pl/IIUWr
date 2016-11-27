using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace IIUWr.Fereol.Interface
{
    public interface ISessionManager
    {
        string MiddlewareToken { get; set; }
        string SessionIdentifier { get; set; }
    }
}
