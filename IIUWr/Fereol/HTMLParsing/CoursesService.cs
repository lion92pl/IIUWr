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
        private readonly IHTTPConnection _connection;
        private const string CoursesPath = "courses/";
        private const string EnrollPath = "records/set-enrolled";
        private const string SetPriorityPath = "records/set-queue-priority";

        private const string SummerHalf = "letni";
        private const string WinterHalf = "zimowy";
        private const string DescriptionForParseError = "<h1>Cannot parse!<h1>";

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
                (?<{RegexGroups.HiddenInput}>{CommonRegexes.HiddenInputPattern})*
                {CommonRegexes.TagsPattern}
            </li>)";

        private static readonly string CoursePattern =
            $@"(?snx)
            (?:<div\s+id=""main\-content"">
                (?<{RegexGroups.HiddenInput}>{CommonRegexes.HiddenInputPattern})*
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
                    <hr>
                    (?<{RegexGroups.Tutorial}>
                    <div\s+class=""tutorial"">
                        <h2>[^<]*</h2>
                        {CommonRegexes.TagsPattern}
                    </div>)*
                </div>
                {CommonRegexes.TagsPattern}
            </div>)";
        
        #endregion

        #region Regeses

        private static readonly Regex SemestersAndCoursesRegex = new Regex(SemestersAndCoursesPattern, RegexOptions.Compiled);
        private static readonly Regex CourseFromListRegex = new Regex(CourseFromListPattern, RegexOptions.Compiled);
        private static readonly Regex CourseRegex = new Regex(CoursePattern, RegexOptions.Compiled);

        #endregion

        public CoursesService(IHTTPConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Semester>> GetSemesters()
        {
            var semesters = await Refresh();
            return semesters?.Keys;
        }

        public async Task<IEnumerable<Course>> GetCourses(Semester semester)
        {
            var semesters = await Refresh();
            return semesters?[semester];
        }

        public async Task<bool> RefreshCourse(Course course)
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
                //TODO after downloading each page I should check if user is still logged in
                ParseCourseFullData(course, match);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<Tutorial>> GetTutorials(Course course)
        {
            string page;
            try
            {
                page = await _connection.GetStringAsync(CoursesPath + course.Path);
            }
            catch
            {
                return null;
            }

            Match match = CourseRegex.Match(page);

            if (match.Success)
            {
                return ParseTutorials(course, match.Groups[RegexGroups.Tutorial].Captures);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> Enroll(Tutorial tutorial, bool enroll)
        {
            var formData = new Dictionary<string, string>
            {
                ["group"] = tutorial.Id.ToString(),
                ["enroll"] = enroll.ToString().ToLower()
            };

            var response = await _connection.Post(EnrollPath, formData);
            return response != null;
        }

        public async Task<bool> SetPriority(Tutorial tutorial, int priority)
        {
            var formData = new Dictionary<string, string>
            {
                ["group"] = tutorial.Id.ToString(),
                ["priority"] = priority.ToString()
            };

            var response = await _connection.Post(SetPriorityPath, formData);
            return response != null;
        }

        private async Task<Dictionary<Semester, List<Course>>> Refresh()
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
                return null;
            }
            
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
            
            return semesters;
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

        private List<Tutorial> ParseTutorials(Course course, CaptureCollection captures)
        {
            List<Tutorial> tutorials = new List<Tutorial>();

            foreach (Capture capture in captures)
            {
                var tutorialsPerType = TutorialParser.ParseTutorials(capture);
                if (tutorialsPerType != null)
                {
                    tutorials.AddRange(tutorialsPerType);
                }
            }

            foreach (var tutorial in tutorials)
            {
                tutorial.Course = course;
            }

            return tutorials;
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

            foreach (Capture hiddenInput in match.Groups[RegexGroups.HiddenInput].Captures)
            {
                var parsed = CommonRegexes.ParseHiddenInput(hiddenInput);
                if (parsed.Item1 == "ajax-course-data")
                {
                    if (parsed.Item2.Contains("is_recording_open&quot;: true"))
                    {
                        course.CanEnroll = true;
                    }
                    if (parsed.Item2.Contains("is_recording_open&quot;: false"))
                    {
                        course.CanEnroll = false;
                    }
                }
            }
            
            foreach (Capture info in match.Groups[RegexGroups.CourseInfo].Captures)
            {
                CourseInfoParser.ParseCourseInfo(course, info);
            }
        }
    }
}
