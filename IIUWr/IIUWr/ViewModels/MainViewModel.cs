using IIUWr.ViewModels.Fereol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(AccountViewModel accountViewModel, SemestersViewModel semestersViewModel, ScheduleViewModel scheduleViewModel)
        {
            AccountViewModel = accountViewModel;
            SemestersViewModel = semestersViewModel;
            ScheduleViewModel = scheduleViewModel;
        }

        public AccountViewModel AccountViewModel { get; }

        public SemestersViewModel SemestersViewModel { get; }

        public ScheduleViewModel ScheduleViewModel { get; }
    }
}
