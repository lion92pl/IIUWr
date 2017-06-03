using IIUWr.Fereol.Interface;
using IIUWr.Utils;
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
            _connection.AuthStatusChanged += AuthStatusChanged;

            Symbol = Windows.UI.Xaml.Controls.Symbol.Contact;

            SetItemName();
        }

        private void AuthStatusChanged(object sender, EventArgs e)
        {
            SetItemName();
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
            NavigationService.Navigate<Views.AccountView, AccountViewModel>();
        }
    }
}
