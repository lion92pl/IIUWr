using IIUWr.ViewModelInterfaces.Fereol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIUWr.Fereol.Model;
using System.ComponentModel;

namespace IIUWr.DesignTime.Fereol
{
    internal class CourseViewModel : ICourseViewModel
    {
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
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
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
