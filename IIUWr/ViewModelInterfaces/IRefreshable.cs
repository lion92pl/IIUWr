using System.ComponentModel;
using System.Threading.Tasks;

namespace IIUWr.ViewModelInterfaces
{
    public interface IRefreshable : INotifyPropertyChanged
    {
        void Refresh();

        bool IsRefreshing { get; set; }
    }
}
