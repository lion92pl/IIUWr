using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.HTMLParsing.Interface;
using System.Runtime.Serialization.Json;

namespace IIUWr.Fereol.WebAPI
{
    public class CoursesService : Interface.ICoursesService
    {
        private const string CoursesPath = @"courses/";
        private const string SemesterInfoPath = @"get_semester_info/";

        private readonly IHTTPConnection _connection;

        public CoursesService(IHTTPConnection connection)
        {
            _connection = connection;
        }

        public Task<bool> Enroll(Tutorial tutorial, bool enroll)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FillCourseDetails(Course course)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Course>> GetCourses(Semester semester)
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(GetCourses)}: Start download");

            string page;
            try
            {
                page = await _connection.GetStringFromAPIAsync(CoursesPath + SemesterInfoPath + semester.Id);
            }
            catch
            {
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"{nameof(GetCourses)}: Finished download, start parsing");

            Models.SemesterInfoResponse response;
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(Models.SemesterInfoResponse));
                var bytes = Encoding.UTF8.GetBytes(page);
                var memoryStream = new System.IO.MemoryStream(bytes);
                response = serializer.ReadObject(memoryStream) as Models.SemesterInfoResponse;
            }
            catch
            {
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"{nameof(GetCourses)}: Finished parsing");

            return response.Courses.Select(c => { var course = (Course)c; course.Semester = semester; return course; });
        }

        public Task<IEnumerable<Semester>> GetSemesters()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tutorial>> GetTutorials(Course course)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetPriority(Tutorial tutorial, int priority)
        {
            throw new NotImplementedException();
        }
    }
}
