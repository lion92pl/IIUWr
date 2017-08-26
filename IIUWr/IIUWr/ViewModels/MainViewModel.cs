using IIUWr.Fereol.Interface;
using IIUWr.ViewModels.Fereol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels
{
    public class MainViewModel : IDisposable
    {
        private readonly IConnection _connection;

        public MainViewModel(AccountViewModel accountViewModel, SemestersViewModel semestersViewModel, ScheduleViewModel scheduleViewModel, IConnection connection)
        {
            AccountViewModel = accountViewModel;
            SemestersViewModel = semestersViewModel;
            ScheduleViewModel = scheduleViewModel;
            
            _connection = connection;
            _connection.AuthStatusChanged += AuthStatusChanged;
        }

        public AccountViewModel AccountViewModel { get; }

        public SemestersViewModel SemestersViewModel { get; }

        public ScheduleViewModel ScheduleViewModel { get; }

        public void Dispose()
        {
            ScheduleViewModel.Dispose();
            AccountViewModel.Dispose();
            _connection.AuthStatusChanged -= AuthStatusChanged;
        }

        private void AuthStatusChanged(object sender, EventArgs e)
        {
            SemestersViewModel.SelectedSemester?.SelectedCourse?.Refresh();
        }
    }
}
