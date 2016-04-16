using IIUWr.ViewModelInterfaces.Fereol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIUWr.Fereol.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IIUWr.DesignTime.Fereol
{
    internal class SemesterViewModel : ISemesterViewModel
    {
        public SemesterViewModel()
        {
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ICourseViewModel> _courses = new ObservableCollection<ICourseViewModel>();
        public ObservableCollection<ICourseViewModel> Courses
        {
            get { return _courses; }
            set
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
            await Task.Delay(1500);
            IsRefreshing = false;
        }
    }
}
