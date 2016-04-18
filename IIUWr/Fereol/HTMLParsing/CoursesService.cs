using IIUWr.Fereol.HTMLParsing.Courses;
using IIUWr.Fereol.HTMLParsing.Interface;
using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.Model.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace IIUWr.Fereol.HTMLParsing
{
    public class CoursesService : ICoursesService
    {
        private IHTTPConnection _connection;
        private const string CoursesPath = @"courses/";

        private const string SummerHalf = "letni";
        private const string WinterHalf = "zimowy";
        private const string DescriptionForParseError = "<h1>Cannot parse!<h1>";
        private DateTimeOffset _lastSemestersUpdate;

        #region Patterns

        private static readonly string SemestersAndCoursesPattern =
            $@"(?snx)
            (?:<div\s+class=""semester""[^>]*>
                <h3>[^<>]*<span>(?<{RegexGroups.Year}>[^\s]*)\s(?<{RegexGroups.YearHalf}>[^<>/]*)</span></h3>
                <input[^<>/]*name=""semester-id""[^<>/]*value=""(?<{RegexGroups.Id}>\d+)""[^<>/]*/>
                <ul\s+class=""courses-list"">
                    (?<{RegexGroups.Course}>
                    <li>
                        {CommonRegexes.TagsPattern}
                    </li>)*
                </ul>
            </div>)";

        private static readonly string CourseFromListPattern =
            $@"(?snx)
            (?:<li>
                <a(?:href=""/courses/(?<{RegexGroups.Path}>(?:\w|\-)+)""|id=""course\-(?<{RegexGroups.Id}>\d+)""|\s*)+>
                    (?<{RegexGroups.Name}>[^<]*)
                </a>
                (?<{RegexGroups.HiddenInput}><input(?:\s*type=""hidden""|\s*name=""[^<]*""|\s*value=""[^<]*""){{3}}\s*/>)*
                {CommonRegexes.TagsPattern}
            </li>)";

        private static readonly string CoursePattern =
            $@"(?snx)
            (?:<div\s+id=""main\-content"">
                {CommonRegexes.TagsPattern}
                <div\s+id=""enr\-course\-view"">
                    <h2>(?<{RegexGroups.Name}>[^<]*)</h2>
                    <div\s+class=""details\s+course\-details"">
                        <table\s+class=""table\-info"">
                            (?<{RegexGroups.CourseInfo}>
                            <tr>
                                <th>[^<]*</th>
                                <td>{CommonRegexes.TagsPattern}</td>
                            </tr>)*
                        </table>
                        <div\s+class=""description"">
                            <h5>[^<]*</h5>
                            (?<{RegexGroups.Description}>{CommonRegexes.TagsPattern})
                        </div>
                        {CommonRegexes.TagsPattern}
                    </div>
                    {CommonRegexes.TagsPattern}
                </div>
                {CommonRegexes.TagsPattern}
            </div>)";
        
        #endregion

        #region Regeses

        private static readonly Regex SemestersAndCoursesRegex = new Regex(SemestersAndCoursesPattern, RegexOptions.Compiled);
        private static readonly Regex CourseFromListRegex = new Regex(CourseFromListPattern, RegexOptions.Compiled);
        private static readonly Regex CourseRegex = new Regex(CoursePattern, RegexOptions.Compiled);

        #endregion

        public CoursesService(Interface.IHTTPConnection connection)
        {
            _connection = connection;
        }

        private Dictionary<Semester, List<Course>> Semesters { get; set; }

        private async Task<bool> Refresh()
        {
            var semesters = new Dictionary<Semester, List<Course>>();

            System.Diagnostics.Debug.WriteLine($"{nameof(Refresh)}: Start download");

            string page;
            try
            {
                page = await _connection.GetStringAsync(CoursesPath);
            }
            catch
            {
                return false;
            }

            _lastSemestersUpdate = DateTimeOffset.Now;

            System.Diagnostics.Debug.WriteLine($"{nameof(Refresh)}: Finished download, start parsing");
            
            foreach (Match match in SemestersAndCoursesRegex.Matches(page))
            {
                if (match.Success)
                {
                    var pair = ParseSemester(match);
                    semesters.Add(pair.Key, pair.Value);
                }
            }

            System.Diagnostics.Debug.WriteLine($"{nameof(Refresh)}: Finished parsing");

            Semesters = semesters;

            return true;
        }

        public async Task<RefreshTime> RefreshCourse(Course course, bool forceRefresh = false)
        {
            string page;
            try
            {
                page = await _connection.GetStringAsync(CoursesPath + course.Path);
            }
            catch
            {
                return RefreshType.Failed;
            }

            Match match = CourseRegex.Match(page);

            if (match.Success)
            {
                ParseCourseFullData(course, match);
                return RefreshType.Full;
            }
            else
            {
                return RefreshType.Failed;
            }
        }

        private KeyValuePair<Semester, List<Course>> ParseSemester(Match match)
        {
            YearHalf yearHalf;

            switch (match.Groups[RegexGroups.YearHalf].Value)
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
                Year = match.Groups[RegexGroups.Year].Value,
                YearHalf = yearHalf,
                Id = int.Parse(match.Groups[RegexGroups.Id].Value)
            };
            var courses = ParseCourses(semester, match.Groups[RegexGroups.Course].Captures);

            return new KeyValuePair<Semester, List<Course>>(semester, courses);
        }

        private List<Course> ParseCourses(Semester semester, CaptureCollection captures)
        {
            List<Course> courses = new List<Course>(captures.Count);

            foreach (Capture capture in captures)
            {
                Course course = ParseCourseBasicData(capture);
                if (course != null)
                {
                    course.Semester = semester;
                    courses.Add(course);
                }
            }
            
            return courses;
        }

        private Course ParseCourseBasicData(Capture capture)
        {
            Match match = CourseFromListRegex.Match(capture.Value);

            if (!match.Success)
            {
                return null;
            }

            Course course = new Course
            {
                Name = match.Groups[RegexGroups.Name].Value,
                Path = match.Groups[RegexGroups.Path].Value,
                Id = int.Parse(match.Groups[RegexGroups.Id].Value)
            };

            foreach (Capture hiddenInput in match.Groups[RegexGroups.HiddenInput].Captures)
            {
                CourseInfoParser.ParseHiddenInput(course, hiddenInput);
            }

            return course;
        }

        private void ParseCourseFullData(Course course, Match match)
        {
            course.Name = match.Groups[RegexGroups.Name].Value;
            course.Description = match.Groups[RegexGroups.Description].Value;
            
            foreach (Capture info in match.Groups[RegexGroups.CourseInfo].Captures)
            {
                CourseInfoParser.ParseCourseInfo(course, info);
            }
        }

        public async Task<Tuple<RefreshTime, IEnumerable<Course>>> GetCourses(Semester semester, bool forceRefresh = false)
        {
            if (Semesters == null || forceRefresh)
            {
                if (!await Refresh())
                {
                    return new Tuple<RefreshTime, IEnumerable<Course>>(RefreshType.Failed, null);
                }
            }
            return new Tuple<RefreshTime, IEnumerable<Course>>(new RefreshTime(RefreshType.Basic, _lastSemestersUpdate), Semesters[semester]);
        }

        public async Task<Tuple<RefreshTime, IEnumerable<Semester>>> GetSemesters(bool forceRefresh = false)
        {
            if (Semesters == null || forceRefresh)
            {
                if (!await Refresh())
                {
                    return new Tuple<RefreshTime, IEnumerable<Semester>>(RefreshType.Failed, null);
                }
            }
            return new Tuple<RefreshTime, IEnumerable<Semester>>(new RefreshTime(RefreshType.Full, _lastSemestersUpdate), Semesters.Keys);
        }
    }
}
