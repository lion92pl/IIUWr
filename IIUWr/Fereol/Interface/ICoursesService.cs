using IIUWr.Fereol.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface ICoursesService
    {
        Task<IEnumerable<Semester>> GetCourses();

        Task RefreshCourse(Course course);
    }
}
