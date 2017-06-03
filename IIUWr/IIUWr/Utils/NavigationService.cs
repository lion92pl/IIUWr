using LionCub.Patterns.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.Utils
{
    public class NavigationService
    {
        public static void Navigate<TView, TViewModel>()
        {
            IoC.Get<Windows.UI.Xaml.Controls.Frame>().Navigate(typeof(TView), IoC.Get<TViewModel>());
        }

        public static void GoBack()
        {
            var frame = IoC.Get<Windows.UI.Xaml.Controls.Frame>();
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
        }
    }
}
