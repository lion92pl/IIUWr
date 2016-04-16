using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IIUWr.ViewModelInterfaces.Fereol
{
    public interface ISemestersViewModel : IRefreshable, INotifyPropertyChanged
    {
        ObservableCollection<ISemesterViewModel> Semesters { get; }

        bool OnlyCurrent { get; set; }
    }
}
