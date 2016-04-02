using IIUWr.Fereol.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface ICoursesService
    {
        Task<IEnumerable<Semester>> GetCourses();

        Task RefreshCourse(Course course);
    }
}
