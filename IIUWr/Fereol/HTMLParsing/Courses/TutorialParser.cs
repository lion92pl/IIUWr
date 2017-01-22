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
    public class TutorialParser
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
                        (<tr\s*>
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
                (<span>[^<]*</span>)+
            </td>
            <td\s+class=""number\s+termLimit"">
                (?<{nameof(Tutorial.Limit)}>[^<]*)
            </td>
            <td\s+class=""number\s+termEnrolledCount"">
                (?<{nameof(Tutorial.Enrolled)}>[^<]*)
            </td>
            <td\s+class=""number\s+termQueuedCount"">
                (?<{nameof(Tutorial.Queue)}>[^<]*)
            </td>";

        private static readonly Regex TutorialsRegex = new Regex(TutorialsPattern, RegexOptions.Compiled);
        private static readonly Regex TutorialRegex = new Regex(TutorialPattern, RegexOptions.Compiled);

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
                var teacherName = match.Groups[RegexGroups.TeacherName].Value.Trim();
                var teacherId = int.Parse(match.Groups[RegexGroups.TeacherId].Value);
                tutorial.Teacher = new Employee
                {
                    Id = teacherId,
                    Name = teacherName
                };

                tutorial.AdvancedGroup = match.Groups[nameof(Tutorial.AdvancedGroup)].Success;
                tutorial.Limit = int.Parse(match.Groups[nameof(Tutorial.Limit)].Value.Trim());
                tutorial.Enrolled = int.Parse(match.Groups[nameof(Tutorial.Enrolled)].Value.Trim());
                tutorial.Queue = int.Parse(match.Groups[nameof(Tutorial.Queue)].Value.Trim());

                return tutorial;
            }
            else
            {
                return null;
            }
        }
    }
}
