using System.ComponentModel;

namespace IIUWr.ViewModelInterfaces
{
    public interface IRefreshable : INotifyPropertyChanged
    {
        void Refresh();

        bool IsRefreshing { get; }
    }
}
