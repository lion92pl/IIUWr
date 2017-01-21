using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IIUWr
{
    /// <summary>Main page</summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = e.Parameter;
        }
    }
}
