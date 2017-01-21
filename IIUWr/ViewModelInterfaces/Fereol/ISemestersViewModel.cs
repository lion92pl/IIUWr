using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IIUWr.ViewModelInterfaces.Fereol
{
    public interface ISemestersViewModel : IRefreshable, INotifyPropertyChanged
    {
        ObservableCollection<ISemesterViewModel> Semesters { get; }
    }
}
