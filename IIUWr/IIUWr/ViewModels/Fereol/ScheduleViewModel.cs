using IIUWr.Fereol.Interface;
using IIUWr.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.HTMLParsing.Interface;

namespace IIUWr.ViewModels.Fereol
{
    public class ScheduleViewModel : IRefreshable, IDisposable
    {
        private readonly IScheduleService _scheduleService;
        private readonly IHTTPConnection _connection;

        private static readonly DayOfWeek[] WorkingDays =
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        public ScheduleViewModel(IScheduleService scheduleService, IHTTPConnection connection)
        {
            _scheduleService = scheduleService;
            _connection = connection;
            _connection.AuthStatusChanged += AuthStatusChanged;
        }
        
        private IList<Tuple<DayOfWeek, IList<ScheduleTutorial>>> _days;
        public IList<Tuple<DayOfWeek, IList<ScheduleTutorial>>> Days
        {
            get { return _days; }
            set
            {
                if (_days != value)
                {
                    _days = value;
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

        public bool CanShow => _connection.AuthStatus != null && _connection.AuthStatus.Authenticated && _connection.AuthStatus.IsStudent;

        public event PropertyChangedEventHandler PropertyChanged;

        public async void Refresh()
        {
            IsRefreshing = true;
            try
            {
                var tutorials = await _scheduleService.GetSchedule();
                var days = new List<Tuple<DayOfWeek, IList<ScheduleTutorial>>>(5);
                foreach (var dayOfWeek in WorkingDays)
                {
                    var dayTutorials = tutorials
                        .Where(tutorial => tutorial.Term.Day == dayOfWeek)
                        .ToList();
                    days.Add(new Tuple<DayOfWeek, IList<ScheduleTutorial>>(dayOfWeek, dayTutorials));
                }
                Days = days;
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public void Dispose()
        {
            _connection.AuthStatusChanged -= AuthStatusChanged;
        }

        private void AuthStatusChanged(object sender, EventArgs e)
        {
            PropertyChanged.Notify(this, nameof(CanShow));
            if (CanShow)
            {
                Refresh();
            }
        }
    }
}
