using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IIUWr.ViewModels.Fereol
{
    public class TutorialViewModel : INotifyPropertyChanged
    {
        public static int[] Priorities { get; } = { 1, 2, 3, 4, 5 };

        private readonly ICoursesService _coursesService;

        public TutorialViewModel(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler EnrollmentStatusChanged;
        
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
                    PropertyChanged.Notify(this, nameof(CanEnroll));
                    PropertyChanged.Notify(this, nameof(CanQueue));
                    PropertyChanged.Notify(this, nameof(IsFull));
                }
            }
        }

        public bool CanEnroll => !Tutorial.IsEnrolled && !IsFull;
        public bool CanQueue => !Tutorial.IsEnrolled && !Tutorial.IsQueued && IsFull;
        public bool IsFull => Tutorial.Limit <= Tutorial.Enrolled;
        
        public async void Enroll()
        {
            await _coursesService.Enroll(Tutorial, true);
            EnrollmentStatusChanged.Invoke(this, EventArgs.Empty);
        }

        public async void UnEnroll()
        {
            await _coursesService.Enroll(Tutorial, false);
            EnrollmentStatusChanged.Invoke(this, EventArgs.Empty);
        }

        //object sender, SelectionChangedEventArgs e
        public async void SetPriority(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            var priority = (int?)e.AddedItems?.SingleOrDefault();
            if (priority.HasValue && priority.Value != Tutorial.Priority)
            {
                await _coursesService.SetPriority(Tutorial, priority.Value);
                EnrollmentStatusChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
