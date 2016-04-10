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

        public async void RefreshDescription()
        {
            await VM.RefreshSelectedCourse();
            ShowDescription();
        }

        private void ShowDescription()
        {
            description.NavigateToString(
                VM?.SelectedCourse?.Description ??
                $@"<h1>Cannot parse!<h1><br /><h3>{VM?.SelectedCourse?.Name ?? "No course selected"}</h3>");
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
                await CoursesService.RefreshSemesters();
            }

            public async Task RefreshSelectedCourse()
            {
                if (SelectedCourse != null)
                {
                    await CoursesService.RefreshCourse(SelectedCourse);
                    PropertyChanged.Notify(this, nameof(SelectedCourse));
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
                    }
                }
            }
        }
    }
}
