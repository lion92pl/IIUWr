using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModelInterfaces.Fereol;
using LionCub.Patterns.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace IIUWr
{
    /// <summary>Main page</summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            VM = IoC.Get<ISemestersViewModel>();
            DataContext = VM;
        }
        
        public ISemestersViewModel VM { get; }
    }
}
