using IIUWr.Fereol.HTMLParsing.Interface;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;

namespace IIUWr.Fereol.HTMLParsing
{
    public class Courses
    {
        private IConnection _connection;

        private const string CoursesPath = @"courses/";

        private const string YearGroup = "year";
        private const string YearHalfGroup = "yearHalf";
        private const string IdGroup = "id";
        private const string CourseGroup = "course";
        private const string PathGroup = "path";
        private const string NameGroup = "name";
        private const string TypeGroup = "type";
        private const string WasEnrolledGroup = "wasEnrolled";
        private const string EnglishGroup = "english";
        private const string ExamGroup = "exam";
        private const string FirstYearGroup = "firstYear";

        private static readonly string SemestersAndCoursesPattern =
            $@"(?:<div\s+class=""semester""[^>]*>)
                (?:<h3>[^<>]*<span>(?<{YearGroup}>[^\s]*)\s(?<{YearHalfGroup}>[^<>/]*)</span></h3>)
                (?:<input[^<>/]*name=""semester-id""[^<>/]*value=""(?<{IdGroup}>\d+)""[^<>/]*/>)
                ((?:<ul\s+class=""courses-list"">
                    (?<{CourseGroup}><li>
                        {TagsPattern}
                    </li>)*
                </ul>))";

        private static readonly string CoursePattern =
            $@"(?:<li>
                (?:<a(?:href=""/courses/(?<{PathGroup}>(?:\w|\-)+)""|id=""course\-(?<{IdGroup}>\d+)""|\s*)+>
                    (?<{NameGroup}>[^<]*)
                </a>)
                (?:<input(?:type=""\w+""|name=""type""|value=""(?<{TypeGroup}>\d+)""|\s*)+/>)
                (?:<input(?:type=""\w+""|name=""wasEnrolled""|value=""(?<{WasEnrolledGroup}>\w+)""|\s*)+/>)
                (?:<input(?:type=""\w+""|name=""english""|value=""(?<{EnglishGroup}>\w+)""|\s*)+/>)
                (?:<input(?:type=""\w+""|name=""exam""|value=""(?<{ExamGroup}>\w+)""|\s*)+/>)
                (?:<input(?:type=""\w+""|name=""suggested_for_first_year""|value=""(?<{FirstYearGroup}>\w+)""|\s*)+/>)
                {TagsPattern}
            </li>)";

        private const string TagsPattern =
            @"(?>
                <!--.*?-->                          |
                <[^>]*/>                            |
                (?<opentag><(?!/)[^>]*[^/]>)        |
                (?<closetag-opentag></[^>]*[^/]>)   |
                [^<>]*)*
            (?(opentag)(?!))";

        private const string SummerHalf = "letni";
        private const string WinterHalf = "zimowy";

        private static readonly Regex SemestersAndCoursesRegex = new Regex(SemestersAndCoursesPattern, RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex CourseRegex = new Regex(CoursePattern, RegexOptions.IgnorePatternWhitespace);

        public Courses(IConnection connection)
        {
            _connection = connection;
        }

        public async void GetData()
        {
            var page = await _connection.GetStringAsync(CoursesPath);

            List<Semester> semesters = new List<Semester>();

            foreach (Match match in SemestersAndCoursesRegex.Matches(page))
            {
                var semester = ParseSemester(match);
                
                semesters.Add(semester);
            }
            var a = semesters.Count;
        }

        private Semester ParseSemester(Match match)
        {
            YearHalf yearHalf;

            switch (match.Groups[YearHalfGroup].Value)
            {
                case SummerHalf:
                    yearHalf = YearHalf.Summer;
                    break;
                case WinterHalf:
                    yearHalf = YearHalf.Winter;
                    break;
                default:
                    yearHalf = YearHalf.Unknown;
                    break;
            }

            Semester semester =  new Semester
            {
                Year = match.Groups[YearGroup].Value,
                YearHalf = yearHalf,
                Id = int.Parse(match.Groups[IdGroup].Value),
            };
            semester.Courses = ParseCourses(semester, match.Groups[CourseGroup].Captures);

            return semester;
        }

        private List<Course> ParseCourses(Semester semester, CaptureCollection captures)
        {
            List<Course> courses = new List<Course>(captures.Count);

            foreach (Capture capture in captures)
            {
                Course course = ParseCourse(capture);
                course.Semester = semester;
                courses.Add(course);
            }

            return courses;
        }

        private Course ParseCourse(Capture capture)
        {
            Match match = CourseRegex.Match(capture.Value);

            //TODO implement
            return new Course
            {
                Name = match.Groups[NameGroup].Value,
                English = bool.Parse(match.Groups[EnglishGroup].Value),
                Exam = bool.Parse(match.Groups[ExamGroup].Value),
                Path = match.Groups[PathGroup].Value,
                Id = int.Parse(match.Groups[IdGroup].Value)
            };
        }
    }
}
