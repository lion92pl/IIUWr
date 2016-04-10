using System;
using System.Text.RegularExpressions;

namespace IIUWr.Fereol.HTMLParsing
{
    public static class CommonRegexes
    {
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
        
        private static readonly string HiddenInputPatern =
            $@"<input(?:\s*type=""hidden""|\s*name=""(?<{RegexGroups.Name}>[^""]*)""|\s*value=""(?<{RegexGroups.Value}>[^""]*)""){{3}}\s*/>";

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
    }
}
