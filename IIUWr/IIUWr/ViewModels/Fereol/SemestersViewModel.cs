using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.Model.Enums;
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

        public SemestersViewModel(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public ObservableCollection<ISemesterViewModel> Semesters { get; private set; }
            = new ObservableCollection<ISemesterViewModel>();
        
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
            var result = await _coursesService.GetSemesters(force);
            RefreshTimes.Set(result.Item1);
            if (result.Item1.IsSuccess && result.Item2 != null)
            {
                foreach (Semester semester in result.Item2)
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
