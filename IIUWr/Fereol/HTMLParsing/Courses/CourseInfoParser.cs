using IIUWr.Fereol.Model;
using System.Text.RegularExpressions;

namespace IIUWr.Fereol.HTMLParsing.Courses
{
    public static class CourseInfoParser
    {
        private static class InfoConsts
        {
            public const string ECTS = "Punkty ECTS";
            public const string Hours = "Liczba godzin";
        }

        private static class HiddenConsts
        {
            public const string Type = "type";
            public const string WasEnrolled = "wasEnrolled";
            public const string English = "english";
            public const string Exam = "exam";
            public const string SuggestedFor1Year = "suggested_for_first_year";
        }

        private static readonly string CourseInfoPattern =
            $@"(?x)
            <tr>
                <th>(?<{RegexGroups.Name}>[^<]*)</th>
                <td>(?<{RegexGroups.Value}>[^<]*)</td>
            </tr>";

        private static readonly string HoursPattern =
            $@"";

        private static readonly Regex CourseInfoRegex = new Regex(CourseInfoPattern, RegexOptions.Compiled);
        private static readonly Regex HoursRegex = new Regex(HoursPattern, RegexOptions.Compiled);

        public static void ParseCourseInfo(Course course, Capture capture)
        {
            Match match = CourseInfoRegex.Match(capture.Value);
            if (match.Success)
            {
                switch (match.Groups[RegexGroups.Name].Value)
                {
                    case InfoConsts.ECTS:
                        course.ECTS = int.Parse(match.Groups[RegexGroups.Value].Value);
                        break;
                    case InfoConsts.Hours:
                        ParseHours(course, capture);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void ParseHiddenInput(Course course, Capture capture)
        {
            var tuple = CommonRegexes.ParseHiddenInput(capture);
            if (tuple != null)
            {
                switch (tuple.Item1)
                {
                    case HiddenConsts.English:
                        course.English = bool.Parse(tuple.Item2);
                        break;
                    case HiddenConsts.Exam:
                        course.Exam = bool.Parse(tuple.Item2);
                        break;
                    case HiddenConsts.SuggestedFor1Year:
                        course.SuggestedFor1Year = bool.Parse(tuple.Item2);
                        break;
                    case HiddenConsts.Type:
                        course.Type = CourseType.Find(int.Parse(tuple.Item2));
                        break;
                    case HiddenConsts.WasEnrolled:
                        course.WasEnrolled = bool.Parse(tuple.Item2);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void ParseHours(Course course, Capture capture)
        {
            //TODO implement
        }
    }
}
