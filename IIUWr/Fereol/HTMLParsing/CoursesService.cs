using IIUWr.Fereol.HTMLParsing.Courses;
using IIUWr.Fereol.Interface;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace IIUWr.Fereol.HTMLParsing
{
    public class CoursesService : ICoursesService
    {
        private Interface.IConnection _connection;
        private const string CoursesPath = @"courses/";

        private const string SummerHalf = "letni";
        private const string WinterHalf = "zimowy";
        
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

        public CoursesService(Interface.IConnection connection)
        {
            _connection = connection;
            Semesters = new ObservableCollection<Semester>();
        }

        public ObservableCollection<Semester> Semesters { get; }

        public async Task RefreshSemesters()
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(RefreshSemesters)}: Start download");

            var page = await _connection.GetStringAsync(CoursesPath);

            System.Diagnostics.Debug.WriteLine($"{nameof(RefreshSemesters)}: Finished download, start parsing");
            
            foreach (Match match in SemestersAndCoursesRegex.Matches(page))
            {
                if (match.Success)
                {
                    var semester = ParseSemester(match);
                    if (Semesters.Contains(semester))
                    {
                        continue;
                    }
                    else
                    {
                        Semesters.Add(semester);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"{nameof(RefreshSemesters)}: Finished parsing");
        }

        /*
        public async Task<IEnumerable<Semester>> GetCourses()
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(GetCourses)}: Start download");

            var page = await _connection.GetStringAsync(CoursesPath);

            System.Diagnostics.Debug.WriteLine($"{nameof(GetCourses)}: Finished download, start parsing");

            List<Semester> semesters = new List<Semester>();

            foreach (Match match in SemestersAndCoursesRegex.Matches(page))
            {
                if (match.Success)
                {
                    semesters.Add(ParseSemester(match));
                }
            }

            System.Diagnostics.Debug.WriteLine($"{nameof(GetCourses)}: Finished parsing");

            return semesters;
        }
        
        public async Task RefreshCoursesForSemester(Semester semester)
        {
            List<Task> tasks = new List<Task>();

            foreach (Course course in semester.Courses)
            {
                tasks.Add(RefreshCourse(course));
            }

            await Task.Run(() => Task.WaitAll(tasks.ToArray()));
        }
        */

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
            semester.Courses = ParseCourses(semester, match.Groups[RegexGroups.Course].Captures);

            return semester;
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
    }
}
