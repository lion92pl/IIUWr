using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.Utils.Refresh;
using IIUWr.ViewModelInterfaces.Fereol;
using System.ComponentModel;

namespace IIUWr.ViewModels.Fereol
{
    public class CourseViewModel : ICourseViewModel
    {
        private readonly ICoursesService _coursesService;
        private readonly RefreshTimesManager _refreshTimesManager;

        public CourseViewModel(ICoursesService coursesService, RefreshTimesManager refreshTimesManager)
        {
            _coursesService = coursesService;
            _refreshTimesManager = refreshTimesManager;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Course _course;
        public Course Course
        {
            get { return _course; }
            set
            {
                if (_course != value)
                {
                    _course = value;
                    PropertyChanged.Notify(this);
                    RefreshTimes = _refreshTimesManager[_course];
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

        private RefreshTimes _refreshTimes;
        public RefreshTimes RefreshTimes
        {
            get { return _refreshTimes; }
            private set
            {
                if (_refreshTimes != value)
                {
                    _refreshTimes = value;
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
            var result = await _coursesService.RefreshCourse(Course, force);
            IsRefreshing = false;
        }
    }
}
