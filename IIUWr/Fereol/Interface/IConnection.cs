using System;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface IConnection
    {
        Task<bool> CheckConnectionAsync();

        Task<bool> LoginAsync(string username, string password);
    }
}
