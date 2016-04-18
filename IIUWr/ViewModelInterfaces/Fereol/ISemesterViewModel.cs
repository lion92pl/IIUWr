using IIUWr.Fereol.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IIUWr.ViewModelInterfaces.Fereol
{
    public interface ISemesterViewModel : IRefreshable, IExpandable, INotifyPropertyChanged
    {
        Semester Semester { get; set; }

        ObservableCollection<ICourseViewModel> Courses { get; }
    }
}
