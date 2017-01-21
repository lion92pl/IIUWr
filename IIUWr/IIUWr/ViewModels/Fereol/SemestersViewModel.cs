using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModelInterfaces.Fereol;
using LionCub.Patterns.DependencyInjection;
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
        
        public void LogIn()
        {
            //IoC.Get<IConnection>().LoginAsync("111111", "testtest");
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
