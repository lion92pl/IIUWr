using IIUWr.Fereol.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIUWr.Fereol.Model;
using IIUWr.Fereol.HTMLParsing.Interface;
using System.Text.RegularExpressions;
using IIUWr.Fereol.Model.Enums;

namespace IIUWr.Fereol.HTMLParsing
{
    public class ScheduleService : IScheduleService
    {
        private const string SchedulePath = @"records/schedule/";

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
            )
            <input(\s+(name=""term""|type=""hidden""|value="""")){{3}}\s*\/>
            )";

        private static readonly string TermPattern =
            $@"(?snx)
            (?:
            <span\s+class=""term"">
                \s*
                (?<{nameof(TimeAndLocation.Day)}>\w+)
                \s*
                (?<{nameof(TimeAndLocation.Start) + nameof(TimeSpan.Hours)}>\d{{1,2}}):(?<{nameof(TimeAndLocation.Start) + nameof(TimeSpan.Minutes)}>\d{{2}})
                \-
                (?<{nameof(TimeAndLocation.End) + nameof(TimeSpan.Hours)}>\d{{1,2}}):(?<{nameof(TimeAndLocation.End) + nameof(TimeSpan.Minutes)}>\d{{2}})
                \s*
            </span>
            <span\s+class=""classroom"">
                sala:\s+(?<{nameof(TimeAndLocation.Location)}>\d*)\s*
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

        public async Task<IEnumerable<ScheduleTutorial>> GetSchedule()
        {
            var tutorials = new List<ScheduleTutorial>();

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
                        var recordMatch = RecordRegex.Match(tutorialCapture.Value);
                        if (recordMatch.Success)
                        {
                            var typeString = recordMatch.Groups[nameof(Tutorial.Type)].Value;
                            var tutorialType = ParseTutorialType(typeString);
                            var termsString = recordMatch.Groups[nameof(Tutorial.Terms)].Value;

                            foreach (Match termMatch in TermRegex.Matches(termsString))
                            {
                                var dayString = termMatch.Groups[nameof(TimeAndLocation.Day)].Value;
                                var startHour = int.Parse(termMatch.Groups[nameof(TimeAndLocation.Start) + nameof(TimeSpan.Hours)].Value);
                                var startMinutes = int.Parse(termMatch.Groups[nameof(TimeAndLocation.Start) + nameof(TimeSpan.Minutes)].Value);
                                var endHour = int.Parse(termMatch.Groups[nameof(TimeAndLocation.End) + nameof(TimeSpan.Hours)].Value);
                                var endMinutes = int.Parse(termMatch.Groups[nameof(TimeAndLocation.End) + nameof(TimeSpan.Minutes)].Value);
                                var locationString = termMatch.Groups[nameof(TimeAndLocation.Location)].Value;

                                var tutorial = new ScheduleTutorial
                                {
                                    Course = course,
                                    Tutorial = new Tutorial
                                    {
                                        Type = tutorialType
                                    },
                                    Term = new TimeAndLocation
                                    {
                                        Day = ParseDayOfWeek(dayString),
                                        Start = new TimeSpan(startHour, startMinutes, 0),
                                        End = new TimeSpan(endHour, endMinutes, 0),
                                        Location = locationString
                                    }
                                };
                                tutorials.Add(tutorial);
                            }
                        }
                    }
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"{nameof(GetSchedule)}: Finished parsing");

            return tutorials;
        }

        private static TutorialType ParseTutorialType(string typeString)
        {
            switch (typeString)
            {
                case "wykład":
                    return TutorialType.Lecture;
                case "repetytorium":
                    return TutorialType.Revision;
                case "pracownia":
                    return TutorialType.Lab;
                case "ćwiczenia":
                    return TutorialType.Class;
                // TODO parse other types
                default:
                    return TutorialType.None;
            }
        }

        private static DayOfWeek ParseDayOfWeek(string dayString)
        {
            switch (dayString)
            {
                case "poniedziałek":
                case "poniedzialek":
                    return DayOfWeek.Monday;
                case "wtorek":
                    return DayOfWeek.Tuesday;
                case "środa":
                case "sroda":
                    return DayOfWeek.Wednesday;
                case "czwartek":
                    return DayOfWeek.Thursday;
                case "piątek":
                case "piatek":
                    return DayOfWeek.Friday;
                default:
                    return DayOfWeek.Saturday;
            }
        }
    }
}
