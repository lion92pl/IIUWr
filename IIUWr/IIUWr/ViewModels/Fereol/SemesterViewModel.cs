using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModels.Interfaces;
using LionCub.Patterns.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IIUWr.ViewModels.Fereol
{
    public class SemesterViewModel : IRefreshable, INotifyPropertyChanged
    {
        private readonly ICoursesService _coursesService;

        public SemesterViewModel(WebAPI.Interface.ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<CourseViewModel> _courses = new ObservableCollection<CourseViewModel>();
        public ObservableCollection<CourseViewModel> Courses
        {
            get { return _courses; }
            private set
            {
                if (_courses != value)
                {
                    _courses = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        private CourseViewModel _selectedCourse;
        public CourseViewModel SelectedCourse
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

        private Semester _semester;
        public Semester Semester
        {
            get { return _semester; }
            set
            {
                if (_semester != value)
                {
                    _semester = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            private set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged.Notify(this);
                }
            }
        }
        
        public async void Refresh()
        {
            IsRefreshing = true;
            var result = await _coursesService.GetCourses(Semester);
            if (result != null)
            {
                foreach (Course course in result)
                {
                    var courseVM = Courses.FirstOrDefault(vm => vm.Course == course);
                    if (courseVM == null)
                    {
                        courseVM = IoC.Get<CourseViewModel>();
                        Courses.Add(courseVM);
                    }
                    courseVM.Course = course;
                }
            }
            IsRefreshing = false;
        }
    }
}
