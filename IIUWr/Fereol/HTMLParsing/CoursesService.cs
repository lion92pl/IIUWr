using IIUWr.Fereol.HTMLParsing.Interface;
using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IIUWr.Fereol.HTMLParsing
{
    public class CoursesService : ICoursesService
    {
        private Interface.IConnection _connection;
        private const string CoursesPath = @"courses/";

        private const string SummerHalf = "letni";
        private const string WinterHalf = "zimowy";

        #region Pattern groups

        private const string YearGroup = "year";
        private const string YearHalfGroup = "yearHalf";
        private const string IdGroup = "id";
        private const string CourseGroup = "course";
        private const string PathGroup = "path";
        private const string NameGroup = "name";
        private const string ValueGroup = "value";
        private const string TypeGroup = "type";
        private const string WasEnrolledGroup = "wasEnrolled";
        private const string EnglishGroup = "english";
        private const string ExamGroup = "exam";
        private const string FirstYearGroup = "firstYear";
        private const string CourseInfoGroup = "courseInfo";

        #endregion

        #region Info names

        private const string ECTSInfo = "Punkty ECTS";

        #endregion

        #region Patterns

        private static readonly string SemestersAndCoursesPattern =
            $@"(?x)
            (?:<div\s+class=""semester""[^>]*>
                <h3>[^<>]*<span>(?<{YearGroup}>[^\s]*)\s(?<{YearHalfGroup}>[^<>/]*)</span></h3>
                <input[^<>/]*name=""semester-id""[^<>/]*value=""(?<{IdGroup}>\d+)""[^<>/]*/>
                <ul\s+class=""courses-list"">
                    (?<{CourseGroup}>
                    <li>
                        {CommonRegexes.TagsPattern}
                    </li>)*
                </ul>
            </div>)";

        private static readonly string CourseFromListPattern =
            $@"(?x)
            (?:<li>
                <a(?:href=""/courses/(?<{PathGroup}>(?:\w|\-)+)""|id=""course\-(?<{IdGroup}>\d+)""|\s*)+>
                    (?<{NameGroup}>[^<]*)
                </a>
                <input(?:type=""\w+""|name=""type""|value=""(?<{TypeGroup}>\d+)""|\s*)+/>
                <input(?:type=""\w+""|name=""wasEnrolled""|value=""(?<{WasEnrolledGroup}>\w+)""|\s*)+/>
                <input(?:type=""\w+""|name=""english""|value=""(?<{EnglishGroup}>\w+)""|\s*)+/>
                <input(?:type=""\w+""|name=""exam""|value=""(?<{ExamGroup}>\w+)""|\s*)+/>
                <input(?:type=""\w+""|name=""suggested_for_first_year""|value=""(?<{FirstYearGroup}>\w+)""|\s*)+/>
                {CommonRegexes.TagsPattern}
            </li>)";

        private static readonly string CoursePattern =
            $@"(?x)
            (?:<div\s+id=""main\-content"">
                {CommonRegexes.TagsPattern}
                <div\s+id=""enr\-course\-view"">
                    <h2>(?<{NameGroup}>[^<]*)</h2>
                    <div\s+class=""details\s+course\-details"">
                        <table\s+class=""table\-info"">
                            (?<{CourseInfoGroup}>
                            <tr>
                                <th>[^<]*</th>
                                <td>[^<]*</td>
                            </tr>)*
                        </table>
                        {CommonRegexes.TagsPattern}
                    </div>
                    {CommonRegexes.TagsPattern}
                </div>
            </div>)";

        private static readonly string CourseInfoPattern =
            $@"(?x)
            <tr>
                <th>(?<{NameGroup}>[^<]*)</th>
                <td>(?<{ValueGroup}>[^<]*)</td>
            </tr>";

        #endregion

        #region Regeses

        private static readonly Regex SemestersAndCoursesRegex = new Regex(SemestersAndCoursesPattern, RegexOptions.Compiled);
        private static readonly Regex CourseFromListRegex = new Regex(CourseFromListPattern, RegexOptions.Compiled);
        private static readonly Regex CourseRegex = new Regex(CoursePattern, RegexOptions.Compiled);
        private static readonly Regex CourseInfoRegex = new Regex(CourseInfoPattern, RegexOptions.Compiled);

        #endregion

        public CoursesService(Interface.IConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Semester>> GetCourses()
        {
            var page = await _connection.GetStringAsync(CoursesPath);

            List<Semester> semesters = new List<Semester>();

            foreach (Match match in SemestersAndCoursesRegex.Matches(page))
            {
                if (match.Success)
                {
                    semesters.Add(ParseSemester(match));
                }
            }

            return semesters;
        }

        public async Task RefreshCourse(Course course)
        {
            var page = await _connection.GetStringAsync(CoursesPath + course.Path);

            Match match = CourseRegex.Match(page);

            if (match.Success)
            {
                ParseCourseFullData(course, match);
            }
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
                Id = int.Parse(match.Groups[IdGroup].Value)
            };
            semester.Courses = ParseCourses(semester, match.Groups[CourseGroup].Captures);

            return semester;
        }

        private List<Course> ParseCourses(Semester semester, CaptureCollection captures)
        {
            List<Course> courses = new List<Course>(captures.Count);

            foreach (Capture capture in captures)
            {
                Course course = ParseCourseBasicData(capture);
                course.Semester = semester;
                courses.Add(course);
            }

            return courses;
        }

        private Course ParseCourseBasicData(Capture capture)
        {
            Match match = CourseFromListRegex.Match(capture.Value);

            //TODO implement
            return new Course
            {
                Name = match.Groups[NameGroup].Value,
                English = bool.Parse(match.Groups[EnglishGroup].Value),
                Exam = bool.Parse(match.Groups[ExamGroup].Value),
                Path = match.Groups[PathGroup].Value,
                Id = int.Parse(match.Groups[IdGroup].Value),
                SuggestedFor1Year = bool.Parse(match.Groups[FirstYearGroup].Value),
                WasEnrolled = bool.Parse(match.Groups[WasEnrolledGroup].Value)
            };
        }

        private void ParseCourseFullData(Course course, Match match)
        {
            course.Name = match.Groups[NameGroup].Value;
            
            foreach (Capture info in match.Groups[CourseInfoGroup].Captures)
            {
                ParseCourseInfo(course, info);
            }
        }

        private void ParseCourseInfo(Course course, Capture capture)
        {
            Match match = CourseInfoRegex.Match(capture.Value);
            if (match.Success)
            {
                switch (match.Groups[NameGroup].Value)
                {
                    case ECTSInfo:
                        course.ECTS = int.Parse(match.Groups[ValueGroup].Value);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
