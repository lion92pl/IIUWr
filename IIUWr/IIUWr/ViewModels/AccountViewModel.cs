using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public LoginViewModel LoginViewModel { get; }

        public async void Logout()
        {
            await _connection.LogoutAsync();
        }

        public void GoBack()
        {
            Utils.NavigationService.GoBack();
        }

        public void Dispose()
        {
            _connection.AuthStatusChanged -= AuthStatusChanged;
        }

        private void AuthStatusChanged(object sender, EventArgs e)
        {
            PropertyChanged.Notify(this, nameof(AuthStatus));
        }
    }
}
