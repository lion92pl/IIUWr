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
using IIUWr.Utils.Refresh;

namespace IIUWr.Fereol.HTMLParsing
{
    public class CoursesService : ICoursesService
    {
        private readonly IHTTPConnection _connection;
        private readonly RefreshTimesManager _refreshTimesManager;
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

        public CoursesService(Interface.IHTTPConnection connection, RefreshTimesManager refreshTimesManager)
        {
            _connection = connection;
            _refreshTimesManager = refreshTimesManager;
        }

        public async Task<IEnumerable<Semester>> GetSemesters(bool forceRefresh = false)
        {
            if (Semesters == null || forceRefresh)
            {
                await Refresh();
            }
            return Semesters?.Keys;
        }

        public async Task<IEnumerable<Course>> GetCourses(Semester semester, bool forceRefresh = false)
        {
            if (Semesters == null || forceRefresh)
            {
                await Refresh();
            }
            return Semesters?[semester];
        }

        public async Task<bool> RefreshCourse(Course course, bool forceRefresh = false)
        {
            string page;
            try
            {
                page = await _connection.GetStringAsync(CoursesPath + course.Path);
            }
            catch
            {
                return false;
            }

            Match match = CourseRegex.Match(page);

            if (match.Success)
            {
                bool auth = CommonRegexes.ParseAuthenticationStatus(page).Authenticated;
                ParseCourseFullData(course, match);
                _refreshTimesManager.Set(course, auth ? RefreshType.LoggedInFull : RefreshType.Full);
                return true;
            }
            else
            {
                _refreshTimesManager.Set(course, RefreshType.Failed);
                return false;
            }
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

            var time = DateTimeOffset.Now;
            bool auth = CommonRegexes.ParseAuthenticationStatus(page).Authenticated;

            System.Diagnostics.Debug.WriteLine($"{nameof(Refresh)}: Finished download, start parsing");
            
            foreach (Match match in SemestersAndCoursesRegex.Matches(page))
            {
                if (match.Success)
                {
                    var pair = ParseSemester(match, new RefreshTime(auth ? RefreshType.LoggedInBasic : RefreshType.Basic));
                    semesters.Add(pair.Key, pair.Value);
                    _refreshTimesManager.Set(pair.Key, new RefreshTime(auth ? RefreshType.LoggedInFull : RefreshType.Full, time));
                }
            }

            System.Diagnostics.Debug.WriteLine($"{nameof(Refresh)}: Finished parsing");

            Semesters = semesters;

            return true;
        }

        private KeyValuePair<Semester, List<Course>> ParseSemester(Match match, RefreshTime refreshTime)
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
            
            var courses = ParseCourses(semester, match.Groups[RegexGroups.Course].Captures, refreshTime);

            return new KeyValuePair<Semester, List<Course>>(semester, courses);
        }

        private List<Course> ParseCourses(Semester semester, CaptureCollection captures, RefreshTime refreshTime)
        {
            List<Course> courses = new List<Course>(captures.Count);

            foreach (Capture capture in captures)
            {
                Course course = ParseCourseBasicData(capture);
                if (course != null)
                {
                    course.Semester = semester;
                    courses.Add(course);
                    _refreshTimesManager.Set(course, refreshTime);
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
    }
}
