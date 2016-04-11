using IIUWr.ViewModelInterfaces.Fereol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using IIUWr.Fereol.Interface;

namespace IIUWr.ViewModels.Fereol
{
    public class SemestersViewModel : ISemestersViewModel
    {
        private readonly ICoursesService _coursesService;

        public SemestersViewModel(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        private ObservableCollection<ISemesterViewModel> _allSemesters;
        private ObservableCollection<ISemesterViewModel> _newestSemester;
        public ObservableCollection<ISemesterViewModel> Semesters { get; }
            = new ObservableCollection<ISemesterViewModel>();

        private bool _onlyCurrent;

        public bool OnlyCurent
        {
            get { return _onlyCurrent; }
            set
            {
                _onlyCurrent = value;
            }
        }


        public async Task<bool> RefreshSemesters()
        {
            await _coursesService.RefreshSemesters();
            return true;
        }
    }
}
