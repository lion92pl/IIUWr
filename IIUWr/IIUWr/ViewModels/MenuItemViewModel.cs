using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace IIUWr.ViewModels
{
    public class MenuItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        private string _glyph;
        public string Glyph
        {
            get { return _glyph; }
            set
            {
                if (_glyph != value)
                {
                    _glyph = value;
                    PropertyChanged.Notify(this);
                }
            }
        }
        
        public Symbol Symbol
        {
            set
            {
                //Glyph = $"{(int)value:X4}";
                Glyph = ((char)value).ToString();
            }
        }

        public virtual async void OnSelected()
        {
            await new Windows.UI.Popups.MessageDialog($"Item with name {Name} selected").ShowAsync();
        }
        
        protected void NotifyPropertyChanged(object source, [System.Runtime.CompilerServices.CallerMemberName] string property = null)
        {
            PropertyChanged.Notify(source, property);
        }
    }
}
