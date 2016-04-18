using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModelInterfaces.Fereol;
using LionCub.Patterns.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IIUWr.ViewModels.Fereol
{
    public class SemesterViewModel : ISemesterViewModel
    {
        private readonly ICoursesService _coursesService;

        public SemesterViewModel(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ICourseViewModel> _courses = new ObservableCollection<ICourseViewModel>();
        public ObservableCollection<ICourseViewModel> Courses
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

        public RefreshTimes RefreshTimes { get; } = new RefreshTimes();

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    if (value)
                    {
                        Refresh();
                    }
                    _isExpanded = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        public void Refresh()
        {
            Refresh(false);
        }

        public void ForceRefresh()
        {
            Refresh(true);
        }

        private async void Refresh(bool force)
        {
            IsRefreshing = true;
            var result = await _coursesService.GetCourses(Semester, force);
            RefreshTimes.Set(result.Item1);
            if (result.Item1.IsSuccess && result.Item2 != null)
            {
                foreach (Course course in result.Item2)
                {
                    var courseVM = Courses.FirstOrDefault(vm => vm.Course == course);
                    if (courseVM == null)
                    {
                        courseVM = IoC.Get<ICourseViewModel>();
                        Courses.Add(courseVM);
                    }
                    courseVM.Course = course;
                    courseVM.RefreshTimes.Set(result.Item1);
                }
            }
            IsRefreshing = false;
        }
    }
}
