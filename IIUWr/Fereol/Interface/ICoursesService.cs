using IIUWr.Fereol.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface ICoursesService
    {
        ObservableCollection<Semester> Semesters { get; }

        Task RefreshSemesters();

        Task RefreshCourse(Course course);
    }
}
