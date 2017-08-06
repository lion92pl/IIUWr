using IIUWr.Fereol.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.HTMLParsing.Interface;
using System.Text.RegularExpressions;

namespace IIUWr.Fereol.HTMLParsing
{
    public class ScheduleService : IScheduleService
    {
        private const string SchedulePath = @"records/schedule";

        #region Patterns

        private static readonly string SchedulePattern =
            $@"(?snx)
            (?:<table\s+(class=""[^""]*""\s*|id=""enr\-schedule\-listByCourse""\s*){{2}}>
                {CommonRegexes.TagsPattern}
            </table>)";

        private static readonly string CourseRecordPattern =
            $@"(?snx)
            (?:
            <tr\s+class=""courseHeader"">
                <td\s+class=""name"">
                    <a\s+href=""(?<{nameof(Course.Path)}>(\w|\-|\/)+)"">
                        \s*(?<{nameof(Course.Name)}>[^<]+?)\s*
                    </a>
                </td>
                <td(\s+(class=""ects""|rowspan=""2"")){{2}}\s*>
                    \s*(?<{nameof(Course.ECTS)}>\d+)\s*
                </td>
            </tr>
            <tr\s+class=""courseDetails"">
                <td><ul>
                    (?<{nameof(Tutorial)}><li>
                        {CommonRegexes.TagsPattern}
                    </li>)+
                </ul></td>
            </tr>
            )";

        private static readonly string RecordPattern =
            $@"(?snx)
            (?:
            <span\s+class=""type"">
                (?<{nameof(Tutorial.Type)}>[^:]*):
            </span>
            (?<{nameof(Tutorial.Terms)}>
                {CommonRegexes.TagsPattern}
            )+
            <input(\s+(name=""term""|type=""hidden""|value="""")){{3}}\s*\/>
            )";

        private static readonly string TermPattern =
            $@"(?snx)
            (?:
            <span\s+class=""term"">
                (?<{nameof(TimeAndLocation.Day)}>\d+)
                (?<{nameof(TimeAndLocation.Start)}>\d{{0,1}}:\d{{2}})-(?<{nameof(TimeAndLocation.End)}>\d{{0,1}}:\d{{2}})
            </span>
            <span\s+class=""classroom"">
                sala:\s+(?<{nameof(TimeAndLocation.Location)}>\d*)
            </span>
            )";

        #endregion

        #region Regexes

        private static readonly Regex ScheduleRegex = new Regex(SchedulePattern, RegexOptions.Compiled);
        private static readonly Regex CourseRecordRegex = new Regex(CourseRecordPattern, RegexOptions.Compiled);
        private static readonly Regex RecordRegex = new Regex(RecordPattern, RegexOptions.Compiled);
        private static readonly Regex TermRegex = new Regex(TermPattern, RegexOptions.Compiled);

        #endregion

        private readonly IHTTPConnection _connection;

        public ScheduleService(IHTTPConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Tutorial>> GetSchedule()
        {
            var tutorials = new List<Tutorial>();

            System.Diagnostics.Debug.WriteLine($"{nameof(GetSchedule)}: Start download");

            string page;
            try
            {
                page = await _connection.GetStringAsync(SchedulePath);
            }
            catch
            {
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"{nameof(GetSchedule)}: Finished download, start parsing");

            var match = ScheduleRegex.Match(page);
            if (match.Success)
            {
                var courseMatches = CourseRecordRegex.Matches(match.Value);
                foreach (Match courseMatch in courseMatches)
                {
                    var path = courseMatch.Groups[nameof(Course.Path)].Value;
                    var name = courseMatch.Groups[nameof(Course.Name)].Value;
                    var ects = int.Parse(courseMatch.Groups[nameof(Course.ECTS)].Value);
                    var tutorialCaptures = courseMatch.Groups[nameof(Tutorial)].Captures;

                    var course = new Course
                    {
                        Name = name,
                        Path = path,
                        ECTS = ects
                    };

                    foreach (Capture tutorialCapture in tutorialCaptures)
                    {
                        tutorials.Add(new Tutorial { Course = course });
                    }
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"{nameof(GetSchedule)}: Finished parsing");

            return tutorials;
        }
    }
}
