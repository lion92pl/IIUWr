using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace IIUWr.ViewModelInterfaces.Fereol
{
    public interface ISemestersViewModel
    {
        ObservableCollection<ISemesterViewModel> Semesters { get; }

        Task<bool> RefreshSemesters();
    }
}
