using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface ICredentialsManager
    {
        string Username { get; set; }
        string Password { get; set; }
    }
}
