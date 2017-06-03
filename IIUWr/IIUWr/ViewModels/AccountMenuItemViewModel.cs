using IIUWr.Fereol.Interface;
using LionCub.Patterns.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels
{
    public class AccountMenuItemViewModel : MenuItemViewModel
    {
        //private readonly IEventAggregator eventAggregator
        private readonly IConnection _connection;

        public AccountMenuItemViewModel(IConnection connection)
        {
            _connection = connection;
            _connection.PropertyChanged += ConnectionPropertyChanged;

            Symbol = Windows.UI.Xaml.Controls.Symbol.Contact;

            SetItemName();
        }

        private void ConnectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IConnection.AuthStatus))
            {
                SetItemName();
            }
        }

        private void SetItemName()
        {
            if (_connection.AuthStatus?.Authenticated ?? false)
            {
                Name = _connection.AuthStatus.Name;
            }
            else
            {
                Name = "NotLoggedIn".Localized("Account");
            }
        }
        
        public override void OnSelected()
        {
            IoC.Get<Windows.UI.Xaml.Controls.Frame>().Navigate(typeof(Views.LoginView), IoC.Get<LoginViewModel>());
        }
    }
}
