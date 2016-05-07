using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModelInterfaces.Fereol;
using LionCub.Patterns.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

namespace IIUWr
{
    /// <summary>Main page</summary>
    public sealed partial class MainPage : Page
    {
        private bool inverted = false;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = e.Parameter;
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //TODO create AttachedProperty for invert
            inverted = !inverted;
            webView.DefaultBackgroundColor = inverted ? Colors.Black : Colors.White;
            int percent = inverted ? 100 : 0;
            string func = $"document.firstChild.style['filter'] = 'invert({percent}%)';";
            await webView.InvokeScriptAsync("eval", new string[] { func });
        }
    }
}
