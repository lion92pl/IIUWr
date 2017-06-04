using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModels.Interfaces;
using LionCub.Patterns.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        private IList<TutorialViewModel> _tutorials;
        public IList<TutorialViewModel> Tutorials
        {
            get { return _tutorials; }
            set
            {
                if (_tutorials != value)
                {
                    _tutorials = value;
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
            var tutorials = await _coursesService.GetTutorials(Course);
            Tutorials = tutorials?.Select(t =>
            {
                var vm = IoC.Get<TutorialViewModel>();
                vm.Tutorial = t;
                vm.EnrollmentStatusChanged += TutorialsEnrollmentStatusChanged;
                return vm;
            }).ToList();
            IsRefreshing = false;
        }

        private void TutorialsEnrollmentStatusChanged(object sender, EventArgs args)
        {
            foreach (var vm in Tutorials)
            {
                vm.EnrollmentStatusChanged -= TutorialsEnrollmentStatusChanged;
            }
            Refresh();
        }
    }
}
