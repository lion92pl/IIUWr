using IIUWr.Fereol.Model;
using System;
using System.ComponentModel;

namespace IIUWr.ViewModelInterfaces
{
    public interface IRefreshable : INotifyPropertyChanged
    {
        void Refresh();
        void ForceRefresh();

        bool IsRefreshing { get; }

        RefreshTimes RefreshTimes { get; }
    }
}
