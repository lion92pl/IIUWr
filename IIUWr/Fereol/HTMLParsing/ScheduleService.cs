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
            (?:<table\s+class=""[^""]*""\s+id=""enr\-schedule\-listByCourse"">
                {CommonRegexes.TagsPattern}
            </table>)";

        private static readonly string CourseRecordPattern =
            $@"(?snx)
            (?:
            <tr\s+class=""courseHeader"">
                <td\s+class=""name"">
                    <a\s+href=""(?<{nameof(Course.Path)}>(?:\w|\-)+)"">
                        (?<{nameof(Course.Name)}>[^<]+)
                    </a>
                </td>
                <td\s+class=""ects""\s+rowspan=""2"">
                    (?<{nameof(Course.ECTS)}>\d+)
                </td>
            </tr>
            <tr\s+class=""courseDetails"">
                <td><ul>
                    (?<{nameof(Tutorial)}><li>
                        {CommonRegexes.TagsPattern}
                    </li>)
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
            <span\s+class=""term"">
                (?<{nameof(TimeAndLocation.Day)}>\d+)
                (?<{nameof(TimeAndLocation.Start)}>\d{{0,1}}:\d{{2}})-(?<{nameof(TimeAndLocation.End)}>\d{{0,1}}:\d{{2}})
            </span>
            <span\s+class=""classroom"">
                sala:\s+(?<{nameof(TimeAndLocation.Location)}>\d*)
            </span>
            )+
            <input\s+name=""term""\s+type=""hidden"" value="""">
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

            }
            
            System.Diagnostics.Debug.WriteLine($"{nameof(GetSchedule)}: Finished parsing");

            return tutorials;
        }
    }
}
