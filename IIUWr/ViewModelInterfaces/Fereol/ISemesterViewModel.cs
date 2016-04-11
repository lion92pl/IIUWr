using IIUWr.Fereol.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IIUWr.ViewModelInterfaces.Fereol
{
    public interface ISemesterViewModel : INotifyPropertyChanged
    {
        Semester Semester { get; }

        ObservableCollection<ICourseViewModel> Courses { get; }

        Task<bool> RefreshCourses();
    }
}
