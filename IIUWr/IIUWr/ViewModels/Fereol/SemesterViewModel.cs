using IIUWr.ViewModelInterfaces.Fereol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIUWr.Fereol.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using IIUWr.Fereol.Interface;
using LionCub.Patterns.DependencyInjection;

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
            set
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
            await _coursesService.Refresh();
            var courses = await _coursesService.GetCourses(Semester);
            if (courses != null)
            {
                foreach (Course course in courses)
                {
                    var courseVM = Courses.FirstOrDefault(vm => vm.Course == course);
                    if (courseVM == null)
                    {
                        courseVM = IoC.Get<ICourseViewModel>();
                        courseVM.Course = course;
                        Courses.Add(courseVM);
                    }
                    else
                    {
                        courseVM.Course = course;
                    }
                }
            }
            IsRefreshing = false;
        }
    }
}
