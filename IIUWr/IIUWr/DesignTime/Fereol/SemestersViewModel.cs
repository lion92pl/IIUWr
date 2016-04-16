using IIUWr.ViewModelInterfaces.Fereol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IIUWr.DesignTime.Fereol
{
    internal class SemestersViewModel : ISemestersViewModel
    {
        public SemestersViewModel()
        {
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

        public ObservableCollection<ISemesterViewModel> Semesters { get; }
            = new ObservableCollection<ISemesterViewModel>()
            {
                new SemesterViewModel
                {
                    Semester = new IIUWr.Fereol.Model.Semester
                    {
                        Year = "2015/16",
                        YearHalf = IIUWr.Fereol.Model.Enums.YearHalf.Summer
                    },
                    Courses = new ObservableCollection<ICourseViewModel>
                    {
                        new CourseViewModel
                        {
                            Course = new IIUWr.Fereol.Model.Course
                            {
                                Name = "Algebra",
                                Type = IIUWr.Fereol.Model.CourseType.Types[0],
                                Exam = true
                            }
                        }
                    }
                }
            };

        private bool _onlyCurrent;
        public bool OnlyCurrent
        {
            get { return _onlyCurrent; }
            set
            {
                if (_onlyCurrent != value)
                {
                    _onlyCurrent = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void Refresh()
        {
            IsRefreshing = true;
            await Task.Delay(1500);
            IsRefreshing = false;
        }
    }
}
