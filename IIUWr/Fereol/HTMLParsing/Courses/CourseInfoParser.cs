using IIUWr.Fereol.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IIUWr.Fereol.HTMLParsing.Courses
{
    public static class CourseInfoParser
    {
        private const string NameGroup = "name";
        private const string ValueGroup = "value";

        private const string ECTSInfo = "Punkty ECTS";

        private static readonly string CourseInfoPattern =
            $@"(?x)
            <tr>
                <th>(?<{NameGroup}>[^<]*)</th>
                <td>(?<{ValueGroup}>[^<]*)</td>
            </tr>";

        private static readonly Regex CourseInfoRegex = new Regex(CourseInfoPattern, RegexOptions.Compiled);

        public static void ParseCourseInfo(Course course, Capture capture)
        {
            Match match = CourseInfoRegex.Match(capture.Value);
            if (match.Success)
            {
                switch (match.Groups[NameGroup].Value)
                {
                    case ECTSInfo:
                        course.ECTS = int.Parse(match.Groups[ValueGroup].Value);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
