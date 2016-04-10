using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace IIUWr.Fereol.HTMLParsing.Interface
{
    public interface ISessionManager
    {
        string SessionCookie { get; set; }
    }
}
