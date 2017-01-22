using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels.Fereol
{
    public class TutorialViewModel : IRefreshable, INotifyPropertyChanged
    {
        private readonly ICoursesService _coursesService;

        public TutorialViewModel(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Tutorial _tutorial;
        public Tutorial Tutorial
        {
            get { return _tutorial; }
            set
            {
                if (_tutorial != value)
                {
                    _tutorial = value;
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
            //var tutorials = await _coursesService.GetTutorials();
            IsRefreshing = false;
        }
    }
}
