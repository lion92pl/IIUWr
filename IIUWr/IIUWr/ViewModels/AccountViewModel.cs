using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using System;
using System.ComponentModel;

namespace IIUWr.ViewModels
{
    public class AccountViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IConnection _connection;

        public event PropertyChangedEventHandler PropertyChanged;

        public AccountViewModel(IConnection connection, LoginViewModel loginViewModel)
        {
            _connection = connection;
            _connection.AuthStatusChanged += AuthStatusChanged;

            LoginViewModel = loginViewModel;
        }

        public AuthenticationStatus AuthStatus => _connection.AuthStatus;

        public string Name
        {
            get
            {
                if (AuthStatus?.Authenticated ?? false)
                {
                    return AuthStatus.Name;
                }
                else
                {
                    return "NotLoggedIn".Localized("Account");
                }
            }
        }

        public LoginViewModel LoginViewModel { get; }

        public async void Logout()
        {
            await _connection.LogoutAsync();
        }
        
        public void Dispose()
        {
            _connection.AuthStatusChanged -= AuthStatusChanged;
        }

        private void AuthStatusChanged(object sender, EventArgs e)
        {
            PropertyChanged.Notify(this, nameof(AuthStatus));
            PropertyChanged.Notify(this, nameof(Name));
        }
    }
}
