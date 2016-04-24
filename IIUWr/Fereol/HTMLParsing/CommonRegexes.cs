using IIUWr.Fereol.Common;
using System;
using System.Text.RegularExpressions;

namespace IIUWr.Fereol.HTMLParsing
{
    public static class CommonRegexes
    {
        public const string BooleanPattern = "(T|t)rue|(F|f)alse";
        
        public const string TagsPattern =
            @"(?>
                <!-- .*? -->                                 |
                <     [^>]*/>                                |
                <hr   [^>]* >                                |
                <br   [^>]* >                                |
                <img  [^>]* >                                |
                (?<opentag>          <(?!/) [^>]* [^/]>)     |
                (?<closetag-opentag> </     [^>]* [^/]>)     |
                [^<>]*
            )*
            (?(opentag)(?!))";

        private static readonly string AuthenticatedPattern =
            $@"(?snx)
            (?:<script\s+type=""text/javascript""[^>]*>\s*
                var\s+user_is_authenticated\s*=\s*(?<{RegexGroups.IsAuthenticated}>{BooleanPattern}),\s*
                user_is_student\s*=\s*(?<{RegexGroups.IsStudent}>{BooleanPattern});\s*
            </script>)";

        private static readonly string HiddenInputPatern =
            $@"<input(?:\s*type=""hidden""|\s*name=""(?<{RegexGroups.Name}>[^""]*)""|\s*value=""(?<{RegexGroups.Value}>[^""]*)""){{3}}\s*/>";

        private static readonly Regex AuthenticatedRegex = new Regex(AuthenticatedPattern, RegexOptions.Compiled);

        private static readonly Regex HiddenInputRegex = new Regex(HiddenInputPatern, RegexOptions.Compiled);

        public static Tuple<string, string> ParseHiddenInput(Capture capture)
        {
            Match match = HiddenInputRegex.Match(capture.Value);
            if (match.Success)
            {
                return new Tuple<string, string>(match.Groups[RegexGroups.Name].Value, match.Groups[RegexGroups.Value].Value);
            }
            return null;
        }

        public static AuthenticationStatus ParseAuthenticationStatus(string page)
        {
            Match match = AuthenticatedRegex.Match(page);
            if (match.Success)
            {
                bool authenticated = bool.Parse(match.Groups[RegexGroups.IsAuthenticated].Value);
                bool student = bool.Parse(match.Groups[RegexGroups.IsStudent].Value);
                return new AuthenticationStatus { Authenticated = authenticated, IsStudent = student };
            }
            return null;
        }
    }
}
