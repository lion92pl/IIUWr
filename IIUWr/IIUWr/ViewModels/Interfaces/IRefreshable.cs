using System.ComponentModel;

namespace IIUWr.ViewModels.Interfaces
{
    public interface IRefreshable : INotifyPropertyChanged
    {
        void Refresh();

        bool IsRefreshing { get; }
    }
}
