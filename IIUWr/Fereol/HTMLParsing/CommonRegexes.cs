using IIUWr.Fereol.Model;
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
                <       [^>]*/>                              |
                <hr     [^>]* >                              |
                <br     [^>]* >                              |
                <img    [^>]* >                              |
                <input  [^>]* >                              |
                (?<opentag>          <(?!/) [^>]* [^/]>)     |
                (?<closetag-opentag> </     [^>]* [^/]>)     |
                [^<>]*
            )*
            (?(opentag)(?!))";

        public const string HiddenInputPattern =
            @"<input(?:\s*(type=(""|')hidden(""|')|name=(""|')[^""']*(""|')|value=(""|')({[^}]*}|[^""']*)(""|'))){3}[^>]*>";

        private static readonly string AuthenticatedPattern =
            $@"(?snx)
            (?:<script\s+type=""text/javascript""[^>]*>\s*
                var\s+user_is_authenticated\s*=\s*(?<{nameof(AuthenticationStatus.Authenticated)}>{BooleanPattern}),\s*
                user_is_student\s*=\s*(?<{nameof(AuthenticationStatus.IsStudent)}>{BooleanPattern});\s*
            </script>)";

        private static readonly string UserNamePattern =
            $@"(?snx)
            (?:<div\s+class=""user-panel""[^>]*>\s*
                [^<]*
                <strong>\s*(?<{nameof(AuthenticationStatus.Name)}>[^<]+?)\s*</strong>
                {TagsPattern}
            </div>)";

        private static readonly string InternalHiddenInputPattern =
            $@"<input(?:\s*(type=(""|')hidden(""|')|name=(""|')(?<{RegexGroups.Name}>[^""']*)(""|')|value=(""|')(?<{RegexGroups.Value}>({{[^}}]*}}|[^""']*))(""|'))){{3}}[^>]*>";

        private static readonly Regex AuthenticatedRegex = new Regex(AuthenticatedPattern, RegexOptions.Compiled);

        private static readonly Regex UserNameRegex = new Regex(UserNamePattern, RegexOptions.Compiled);

        private static readonly Regex HiddenInputRegex = new Regex(InternalHiddenInputPattern, RegexOptions.Compiled);

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
            AuthenticationStatus result = null;
            Match match = AuthenticatedRegex.Match(page);
            if (match.Success)
            {
                var authenticated = bool.Parse(match.Groups[nameof(AuthenticationStatus.Authenticated)].Value);
                var isStudent = bool.Parse(match.Groups[nameof(AuthenticationStatus.IsStudent)].Value);
                result = new AuthenticationStatus { Authenticated = authenticated, IsStudent = isStudent };

                match = UserNameRegex.Match(page);
                if (match.Success)
                {
                    result.Name = match.Groups[nameof(AuthenticationStatus.Name)].Value;
                }
            }
            return result;
        }
    }
}
