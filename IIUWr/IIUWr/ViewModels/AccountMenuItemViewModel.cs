using LionCub.Patterns.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels
{
    public class AccountMenuItemViewModel : MenuItemViewModel
    {
        //private readonly IEventAggregator eventAggregator

        public AccountMenuItemViewModel()
        {
            Symbol = Windows.UI.Xaml.Controls.Symbol.Contact;
            LoggedIn = false;
        }

        private bool _loggedIn;
        public bool LoggedIn
        {
            get { return _loggedIn; }
            set
            {
                if (_loggedIn != value)
                {
                    _loggedIn = value;
                    NotifyPropertyChanged(this);
                }
                if (!value)
                {
                    Name = "NotLoggedIn".Localized("Account");
                }
            }
        }

        public override void OnSelected()
        {
            IoC.Get<Windows.UI.Xaml.Controls.Frame>().Navigate(typeof(Views.LoginView), IoC.Get<LoginViewModel>());
        }
    }
}
