using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModels.Interfaces;
using LionCub.Patterns.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IIUWr.ViewModels.Fereol
{
    public class SemestersViewModel : IRefreshable, INotifyPropertyChanged
    {
        private readonly ICoursesService _coursesService;

        public SemestersViewModel(ICoursesService coursesService, FiltersViewModel filtersViewModel)
        {
            _coursesService = coursesService;
            Filters = filtersViewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FiltersViewModel Filters { get; set; }

        private ObservableCollection<SemesterViewModel> _semesters = new ObservableCollection<SemesterViewModel>();
        public ObservableCollection<SemesterViewModel> Semesters
        {
            get { return _semesters; }
            private set
            {
                if (_semesters != value)
                {
                    _semesters = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        private SemesterViewModel _selectedSemester;
        public SemesterViewModel SelectedSemester
        {
            get { return _selectedSemester; }
            set
            {
                if (_selectedSemester != value)
                {
                    _selectedSemester = value;
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
            var result = await _coursesService.GetSemesters();
            if (result != null)
            {
                foreach (Semester semester in result)
                {
                    var semesterVM = Semesters.FirstOrDefault(vm => vm.Semester == semester);
                    if (semesterVM == null)
                    {
                        semesterVM = IoC.Get<SemesterViewModel>();
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

            if (Semesters.Any() && (SelectedSemester == null || !Semesters.Contains(SelectedSemester)))
            {
                SelectedSemester = Semesters[0];
            }
            SelectedSemester?.Refresh();
        }
    }
}
