using IIUWr.Fereol.Model;
using System.Collections.Generic;
using System.ComponentModel;
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
            VM = new ViewModel();
        }

        public async Task RefreshDescription()
        {
            await VM.RefreshSelectedCourse();
            description.NavigateToString(
                VM?.SelectedCourse?.Description ??
                $@"<h1>Cannot parse!<h1><br /><h3>{VM?.SelectedCourse?.Name ?? "No course selected"}</h3>");
        }

        public ViewModel VM { get; set; }

        public class ViewModel : INotifyPropertyChanged
        {
            private Fereol.HTMLParsing.CoursesService _coursesService;

            public ViewModel()
            {
                _coursesService = new Fereol.HTMLParsing.CoursesService(new Fereol.HTMLParsing.Connection());
            }

            public async Task RefreshSemesters()
            {
                Semesters = await _coursesService.GetCourses();
            }

            public async Task RefreshSelectedCourse()
            {
                if (SelectedCourse != null)
                {
                    await _coursesService.RefreshCourse(SelectedCourse);
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private IEnumerable<Semester> _semesters;
            public IEnumerable<Semester> Semesters
            {
                get { return _semesters; }
                set
                {
                    if (_semesters != value)
                    {
                        _semesters = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Semesters))); 
                    }
                }
            }

            private Semester _selectedSemester;
            public Semester SelectedSemester
            {
                get { return _selectedSemester; }
                set
                {
                    if (_selectedSemester != value)
                    {
                        _selectedSemester = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSemester)));
                    }
                }
            }

            private Course _selectedCourse;
            public Course SelectedCourse
            {
                get { return _selectedCourse; }
                set
                {
                    if (_selectedCourse != value)
                    {
                        _selectedCourse = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCourse)));
                    }
                }
            }
        }
    }
}
