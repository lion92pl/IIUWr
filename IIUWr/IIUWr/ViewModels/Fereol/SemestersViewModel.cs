using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.Model.Enums;
using IIUWr.Utils.Refresh;
using IIUWr.ViewModelInterfaces.Fereol;
using LionCub.Patterns.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IIUWr.ViewModels.Fereol
{
    public class SemestersViewModel : ISemestersViewModel
    {
        private readonly ICoursesService _coursesService;
        private readonly RefreshTimesManager _refreshTimesManager;

        public SemestersViewModel(ICoursesService coursesService, RefreshTimesManager refreshTimesManager)
        {
            _coursesService = coursesService;
            _refreshTimesManager = refreshTimesManager;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ISemesterViewModel> _semesters = new ObservableCollection<ISemesterViewModel>();
        public ObservableCollection<ISemesterViewModel> Semesters
        {
            get { return _semesters; }
            private set
            {
                if (_semesters != value)
                {
                    _semesters = value;
                    PropertyChanged.Notify(this);
                    RefreshTimes = _refreshTimesManager[_semesters];
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

        public void LogIn()
        {
            IoC.Get<IConnection>().Login("111111", "testtest");
        }

        private async void Refresh(bool force)
        {
            IsRefreshing = true;
            var result = await _coursesService.GetSemesters(force);
            if (result != null)
            {
                foreach (Semester semester in result)
                {
                    var semesterVM = Semesters.FirstOrDefault(vm => vm.Semester == semester);
                    if (semesterVM == null)
                    {
                        semesterVM = IoC.Get<ISemesterViewModel>();
                        semesterVM.Semester = semester;
                        Semesters.Add(semesterVM);
                    }
                    else
                    {
                        semesterVM.Semester = semester;
                    }
                }
            }
            IsRefreshing = false;
        }
    }
}
