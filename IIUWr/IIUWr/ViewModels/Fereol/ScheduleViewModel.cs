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
    public class ScheduleViewModel : IRefreshable
    {
        private readonly IScheduleService _scheduleService;
        private readonly IHTTPConnection _connection;

        public ScheduleViewModel(IScheduleService scheduleService, IHTTPConnection connection)
        {
            _scheduleService = scheduleService;
            _connection = connection;
            _connection.AuthStatusChanged += AuthStatusChanged;
        }
        
        private IList<Tutorial> _tutorials;
        public IList<Tutorial> Tutorials
        {
            get { return _tutorials; }
            set
            {
                if (_tutorials != value)
                {
                    _tutorials = value;
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
            var tutorials = await _scheduleService.GetSchedule();
            Tutorials = tutorials.ToList();
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
