using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(AccountMenuItemViewModel accountItem)
        {
            AccountItem = accountItem;
            Items = new[]
            {
                new MenuItemViewModel { Name = "Przedmioty", Glyph = "\uF168" },
                new MenuItemViewModel { Name = "Plan zajęć", Symbol = Windows.UI.Xaml.Controls.Symbol.CalendarWeek }
            };
            BottomItems = new[] { AccountItem, new MenuItemViewModel { Name = "Test", Symbol = Windows.UI.Xaml.Controls.Symbol.Add } };
        }

        public AccountMenuItemViewModel AccountItem { get; }

        public IEnumerable<MenuItemViewModel> Items { get; }
        public IEnumerable<MenuItemViewModel> BottomItems { get; }

        private MenuItemViewModel _selectedItem;
        public MenuItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = null;
                    PropertyChanged.Notify(this);
                    _selectedItem = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

    }
}
