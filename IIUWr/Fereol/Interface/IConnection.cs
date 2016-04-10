using System;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface IConnection
    {
        Uri FereolBaseUri { get; set; }

        Task<bool> CheckConnection();

        Task<bool> Login(string username, string password);
    }
}
