using IIUWr.Fereol.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface ICoursesService
    {
        Task<Tuple<RefreshTime, IEnumerable<Semester>>> GetSemesters(bool forceRefresh = false);
        Task<Tuple<RefreshTime, IEnumerable<Course>>> GetCourses(Semester semester, bool forceRefresh = false);
        Task<RefreshTime> RefreshCourse(Course course, bool forceRefresh = false);
    }
}
