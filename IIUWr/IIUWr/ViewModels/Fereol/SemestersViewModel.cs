using IIUWr.ViewModelInterfaces.Fereol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using LionCub.Patterns.DependencyInjection;
using System.ComponentModel;

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

        private ObservableCollection<ISemesterViewModel> _allSemesters;
        public ObservableCollection<ISemesterViewModel> Semesters { get; private set; }
            = new ObservableCollection<ISemesterViewModel>();

        private bool _onlyCurrent;
        public bool OnlyCurrent
        {
            get { return _onlyCurrent; }
            set
            {
                if (_onlyCurrent != value)
                {
                    _onlyCurrent = value;
                    ApplyOnlyCurrent(_onlyCurrent);
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
            var semesters = await _coursesService.GetSemesters();
            if (semesters != null)
            {
                _allSemesters = new ObservableCollection<ISemesterViewModel>();
                //TODO better handling of OnlyCurrent
                foreach (Semester semester in OnlyCurrent ? semesters.Take(1) : semesters)
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
                    _allSemesters.Add(semesterVM);
                }
            }
            IsRefreshing = false;
        }

        private void ApplyOnlyCurrent(bool onlyCurrent)
        {
            if (onlyCurrent)
            {
                while (Semesters.Count > 1)
                {
                    Semesters.RemoveAt(1);
                }
            }
            else
            {
                for (int i = Semesters.Count; i < _allSemesters.Count; i++)
                {
                    Semesters.Add(_allSemesters[i]);
                }
            }
        }
    }
}
