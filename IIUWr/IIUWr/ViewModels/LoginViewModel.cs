using IIUWr.Fereol.Interface;
using System.ComponentModel;
using Windows.UI.Xaml.Input;

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
            if (!string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password))
            {
                var loggedIn = await _connection.LoginAsync(Login, Password);
                if (loggedIn)
                {
                    return;
                }
            }
            InvalidCredentials = true;
        }

        public void TryLoginByEnter(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key == Windows.System.VirtualKey.Enter)
            {
                TryLoginAsync();
            }
        }
    }
}
