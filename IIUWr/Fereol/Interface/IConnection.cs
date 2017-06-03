using IIUWr.Fereol.Model;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface IConnection : INotifyPropertyChanged
    {
        Task<bool> CheckConnectionAsync();

        Task<bool> LoginAsync(string username, string password);

        Task<bool> LogoutAsync();

        AuthenticationStatus AuthStatus { get; }
    }
}
