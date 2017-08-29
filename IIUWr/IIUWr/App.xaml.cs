using IIUWr.ViewModels;
using IIUWr.Views;
using LionCub.Patterns.DependencyInjection;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace IIUWr
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.
        /// </summary>
        public App()
        {
            InitializeComponent();
            
            ConfigureIoC.All();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.IsBindingTracingEnabled = true;
            }
#endif
            
            MainView rootView = Window.Current.Content as MainView;

            if (rootView == null)
            {
                rootView = new MainView
                {
                    DataContext = IoC.Get<MainViewModel>()
                };

                Window.Current.Content = rootView;
            }

            Window.Current.Activate();
        }
    }
}