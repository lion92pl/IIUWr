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
            VM = new ViewModel();
        }

        public async Task RefreshDescription()
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
            private ICoursesService _coursesService;
            private int? _lastSemesterId;
            private int? _lastCourseId;

            public ViewModel()
            {
                _coursesService = IoC.Get<ICoursesService>();
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCourse)));
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
                        SelectedSemester = _semesters?.FirstOrDefault(s => s.Id == _lastSemesterId)
                                        ?? _semesters?.FirstOrDefault();
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
                        if (_selectedSemester != null)
                        {
                            _lastSemesterId = _selectedSemester.Id;
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSemester)));
                        SelectedCourse = _selectedSemester?.Courses?.FirstOrDefault(c => c.Id == _lastCourseId)
                                      ?? _selectedSemester?.Courses?.FirstOrDefault();
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
                        if (_selectedCourse != null)
                        {
                            _lastCourseId = _selectedCourse.Id;
                        }
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCourse)));
                    }
                }
            }
        }
    }
}
