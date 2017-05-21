using IIUWr.Fereol.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IConnection _connection;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel(IConnection connection)
        {
            _connection = connection;
        }

        private string _login;
        public string Login
        {
            get { return _login; }
            set
            {
                if (_login != value)
                {
                    _login = value;
                    PropertyChanged.Notify(this);
                    InvalidCredentials = false;
                }
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    PropertyChanged.Notify(this);
                    InvalidCredentials = false;
                }
            }
        }

        private bool _invalidCredentials;
        public bool InvalidCredentials
        {
            get { return _invalidCredentials; }
            set
            {
                if (_invalidCredentials != value)
                {
                    _invalidCredentials = value;
                    PropertyChanged.Notify(this);
                }
            }
        }
        
        public async void TryLoginAsync()
        {
            var loggedIn = await _connection.LoginAsync(Login, Password);
            if (loggedIn)
            {
                await new Windows.UI.Popups.MessageDialog($"Logged in as {Login}").ShowAsync();
            }
            else
            {
                InvalidCredentials = true;
            }
        }
    }
}
