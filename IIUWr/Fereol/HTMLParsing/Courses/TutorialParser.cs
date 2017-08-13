using IIUWr.Fereol.Model;
using IIUWr.Fereol.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IIUWr.Fereol.HTMLParsing.Courses
{
    public static class TutorialParser
    {
        private static readonly string TutorialsPattern =
            $@"(?x)
            <div\s+class=""tutorial"">
                <h2>[^<]*</h2>
                
                (?<{RegexGroups.TutorialType}>{CommonRegexes.HiddenInputPattern})
                <table\s+class=""zebra-striped"">
                    <thead>
                        {CommonRegexes.TagsPattern}
                    </thead>
                    <tbody>
                        (<tr\s*(class=""signed"")?\s*>
                            (?<{RegexGroups.Tutorial}>{CommonRegexes.TagsPattern})
                        </tr>)*
                    </tbody>
                </table>
                {CommonRegexes.TagsPattern}
            </div>";

        private static readonly string TutorialPattern =
            $@"(?x)
            <td>
                <a(\s+(href=""/users/profile/employee/(?<{RegexGroups.TeacherId}>(\d+))""|class=""person"")){{2}}>(?<{RegexGroups.TeacherName}>[^<]*)</a>
                (<br\s*/?><span\s+class=""small"">(?<{nameof(Tutorial.AdvancedGroup)}>[^<]*)</span>)?
            </td>
            <td\s+class=""term"">
                (?<{nameof(Tutorial.Terms)}><span>[^<]*</span>)+
            </td>
            <td\s+class=""number\s+termLimit"">
                ((?<{nameof(Tutorial.LimitInterdisciplinary)}>[^<]*)\+)?(?<{nameof(Tutorial.Limit)}>[^<]*)
            </td>
            <td\s+class=""number\s+termEnrolledCount"">
                ((?<{nameof(Tutorial.EnrolledInterdisciplinary)}>[^<]*)\+)?(?<{nameof(Tutorial.Enrolled)}>[^<]*)
            </td>
            <td\s+class=""number\s+termQueuedCount"">
                (?<{nameof(Tutorial.Queue)}>[^<]*)
            </td>
            <td\s+class=""controls"">
                (?<{nameof(Tutorial.Id)}>{CommonRegexes.HiddenInputPattern})
                (?<{RegexGroups.JSON}>{CommonRegexes.HiddenInputPattern})
                (?<{nameof(Tutorial.IsEnrolled)}>{CommonRegexes.HiddenInputPattern})
                {CommonRegexes.TagsPattern}
            </td>
            (<td\s+class=""priority"">
                {CommonRegexes.TagsPattern}
                <span>(?<{nameof(Tutorial.Priority)}>\d)</span>
                {CommonRegexes.TagsPattern}
            </td>)?";

        private static readonly string TermPattern =
            $@"(?x)
            <span>
                (?<{nameof(TimeAndLocation.Day)}>\w+)\s
                (?<{nameof(TimeAndLocation.Start) + nameof(TimeSpan.Hours)}>\d{{1,2}}):(?<{nameof(TimeAndLocation.Start) + nameof(TimeSpan.Minutes)}>\d{{2}})
                \-
                (?<{nameof(TimeAndLocation.End) + nameof(TimeSpan.Hours)}>\d{{1,2}}):(?<{nameof(TimeAndLocation.End) + nameof(TimeSpan.Minutes)}>\d{{2}})\s
                \(s.((?<{nameof(TimeAndLocation.Location)}>\d+),?)+\)
            </span>";
        
        private static readonly Regex TutorialsRegex = new Regex(TutorialsPattern, RegexOptions.Compiled);
        private static readonly Regex TutorialRegex = new Regex(TutorialPattern, RegexOptions.Compiled);
        private static readonly Regex TermRegex = new Regex(TermPattern, RegexOptions.Compiled);

        public static IEnumerable<Tutorial> ParseTutorials(Capture capture)
        {
            var match = TutorialsRegex.Match(capture.Value);
            if (match.Success)
            {
                var tutorials = new List<Tutorial>(match.Groups[RegexGroups.Tutorial].Captures.Count);

                var tutorialTypeHiddenInput = CommonRegexes.ParseHiddenInput(match.Groups[RegexGroups.TutorialType]);
                var type = (TutorialType)int.Parse(tutorialTypeHiddenInput.Item2);

                foreach (Capture tutorial in match.Groups[RegexGroups.Tutorial].Captures)
                {
                    var parsedTutorial = ParseTutorial(tutorial);
                    if (parsedTutorial == null)
                    {
                        continue;
                    }

                    parsedTutorial.Type = type;
                    yield return parsedTutorial;
                }                
            }
            else
            {
                yield break;
            }
        }

        private static Tutorial ParseTutorial(Capture capture)
        {
            var match = TutorialRegex.Match(capture.Value);
            if (match.Success)
            {
                var tutorial = new Tutorial();

                var idHiddenInput = CommonRegexes.ParseHiddenInput(match.Groups[nameof(Tutorial.Id)]);
                tutorial.Id = int.Parse(idHiddenInput.Item2);

                var teacherName = match.Groups[RegexGroups.TeacherName].Value.Trim();
                var teacherId = int.Parse(match.Groups[RegexGroups.TeacherId].Value);
                tutorial.Teacher = new Employee
                {
                    Id = teacherId,
                    Name = teacherName
                };

                tutorial.Terms = ParseTerms(match.Groups[nameof(Tutorial.Terms)].Captures).ToList();
                
                tutorial.AdvancedGroup = match.Groups[nameof(Tutorial.AdvancedGroup)].Success;
                tutorial.Limit = int.Parse(match.Groups[nameof(Tutorial.Limit)].Value.Trim());
                tutorial.Enrolled = int.Parse(match.Groups[nameof(Tutorial.Enrolled)].Value.Trim());
                tutorial.Queue = int.Parse(match.Groups[nameof(Tutorial.Queue)].Value.Trim());

                if (match.Groups[nameof(Tutorial.LimitInterdisciplinary)].Success)
                {
                    tutorial.LimitInterdisciplinary = int.Parse(match.Groups[nameof(Tutorial.LimitInterdisciplinary)].Value.Trim());
                }

                if (match.Groups[nameof(Tutorial.EnrolledInterdisciplinary)].Success)
                {
                    tutorial.EnrolledInterdisciplinary = int.Parse(match.Groups[nameof(Tutorial.EnrolledInterdisciplinary)].Value.Trim());
                }

                if (match.Groups[nameof(Tutorial.IsEnrolled)].Success)
                {
                    var isEnrolledHiddenInput = CommonRegexes.ParseHiddenInput(match.Groups[nameof(Tutorial.IsEnrolled)]);
                    tutorial.IsEnrolled = bool.Parse(isEnrolledHiddenInput.Item2);
                }

                if (match.Groups[nameof(Tutorial.Priority)].Success)
                {
                    tutorial.IsQueued = true;
                    tutorial.Priority = int.Parse(match.Groups[nameof(Tutorial.Priority)].Value.Trim());
                }

                return tutorial;
            }
            else
            {
                return null;
            }
        }

        private static IEnumerable<TimeAndLocation> ParseTerms(CaptureCollection captures)
        {
            var result = new List<TimeAndLocation>();
            foreach (Capture capture in captures)
            {
                var match = TermRegex.Match(capture.Value);
                if (match.Success)
                {
                    var dayString = match.Groups[nameof(TimeAndLocation.Day)].Value;
                    var startHour = int.Parse(match.Groups[nameof(TimeAndLocation.Start) + nameof(TimeSpan.Hours)].Value);
                    var startMinutes = int.Parse(match.Groups[nameof(TimeAndLocation.Start) + nameof(TimeSpan.Minutes)].Value);
                    var endHour = int.Parse(match.Groups[nameof(TimeAndLocation.End) + nameof(TimeSpan.Hours)].Value);
                    var endMinutes = int.Parse(match.Groups[nameof(TimeAndLocation.End) + nameof(TimeSpan.Minutes)].Value);

                    var locationCaptures = match.Groups[nameof(TimeAndLocation.Location)].Captures;
                    var locationString = string.Join(", ", locationCaptures.OfType<Capture>().Select(c => c.Value));

                    var term = new TimeAndLocation
                    {
                        Day = ParseDayOfWeek(dayString),
                        Start = new TimeSpan(startHour, startMinutes, 0),
                        End = new TimeSpan(endHour, endMinutes, 0),
                        Location = locationString
                    };
                    result.Add(term);
                }
            }
            return result;
        }

        private static DayOfWeek ParseDayOfWeek(string dayString)
        {
            switch (dayString)
            {
                case "pn":
                    return DayOfWeek.Monday;
                case "wt":
                    return DayOfWeek.Tuesday;
                case "śr":
                    return DayOfWeek.Wednesday;
                case "cz":
                    return DayOfWeek.Thursday;
                case "pt":
                    return DayOfWeek.Friday;
                default:
                    return DayOfWeek.Sunday;
            }
        }
    }
}
