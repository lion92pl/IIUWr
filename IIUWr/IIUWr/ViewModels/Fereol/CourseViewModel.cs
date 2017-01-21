using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModels.Interfaces;
using System.ComponentModel;

namespace IIUWr.ViewModels.Fereol
{
    public class CourseViewModel : IRefreshable, INotifyPropertyChanged
    {
        private readonly ICoursesService _coursesService;

        public CourseViewModel(ICoursesService coursesService)
        {
            _coursesService = coursesService;
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
            var result = await _coursesService.RefreshCourse(Course);
            IsRefreshing = false;
        }
    }
}
