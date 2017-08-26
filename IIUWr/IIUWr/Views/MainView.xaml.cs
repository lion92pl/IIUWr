using LionCub.Patterns.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IIUWr.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        private const string OneColumnVisualState = "oneColumn";

        private readonly SystemNavigationManager _navigationManager;

        public MainView()
        {
            this.InitializeComponent();
            _navigationManager = SystemNavigationManager.GetForCurrentView();
            courseDetails.RegisterPropertyChangedCallback(VisibilityProperty, HandleCourseDetailsVisibility);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = e.Parameter;
        }

        private void VisualStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            HandleCanGoBack();
        }

        private void HandleCanGoBack()
        {
            var canGoBack = visualStates.CurrentState?.Name == OneColumnVisualState
                     && courseDetails.Visibility == Visibility.Visible;

            _navigationManager.AppViewBackButtonVisibility = canGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
            if (canGoBack)
            {
                _navigationManager.BackRequested += BackRequested;
            }
            else
            {
                _navigationManager.BackRequested -= BackRequested;
            }
        }

        private void HandleCourseDetailsVisibility(DependencyObject sender, DependencyProperty property)
        {
            HandleCanGoBack();
        }

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            //TODO Clear selected course instead
            (DataContext as ViewModels.MainViewModel).SemestersViewModel.SelectedSemester.SelectedCourse = null;
            e.Handled = true;
        }

    }
}
