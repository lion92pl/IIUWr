using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
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
            VM = IoC.Get<ViewModel>();
        }
        
        public ViewModel VM { get; set; }

        public class ViewModel : INotifyPropertyChanged
        {
            public ViewModel(ICoursesService coursesService)
            {
                CoursesService = coursesService;
            }

            public ICoursesService CoursesService { get; }

            public async void RefreshSemesters()
            {
                IsRefreshingSemesters = true;
                await CoursesService.RefreshSemesters();
                IsRefreshingSemesters = false;
            }

            public async void RefreshSelectedCourse()
            {
                if (SelectedCourse != null)
                {
                    IsRefreshingCourse = true;
                    await CoursesService.RefreshCourse(SelectedCourse);
                    PropertyChanged.Notify(this, nameof(SelectedCourse));
                    IsRefreshingCourse = false;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            
            private Course _selectedCourse;
            public Course SelectedCourse
            {
                get { return _selectedCourse; }
                set
                {
                    if (_selectedCourse != value)
                    {
                        _selectedCourse = value;
                        PropertyChanged.Notify(this);
                        RefreshSelectedCourse();
                    }
                }
            }

            private bool _isRefreshingCourse;
            public bool IsRefreshingCourse
            {
                get { return _isRefreshingCourse; }
                set
                {
                    if (_isRefreshingCourse != value)
                    {
                        _isRefreshingCourse = value;
                        PropertyChanged.Notify(this);
                    }
                }
            }

            private bool _isRefreshingSemesters;
            public bool IsRefreshingSemesters
            {
                get { return _isRefreshingSemesters; }
                set
                {
                    if (_isRefreshingSemesters != value)
                    {
                        _isRefreshingSemesters = value;
                        PropertyChanged.Notify(this);
                    }
                }
            }
        }
    }
}
