using System;
using System.Text.RegularExpressions;

namespace IIUWr.Fereol.HTMLParsing
{
    public static class CommonRegexes
    {
        public const string TagsPattern =
            @"(?>
                <!--.*?-->                          |
                <[^>]*/>                            |
                (?<opentag><(?!/)[^>]*[^/]>)        |
                (?<closetag-opentag></[^>]*[^/]>)   |
                [^<>]*)*
            (?(opentag)(?!))";

        //TODO: check what's wrong, correct, use
        public const string TagsPatternNew =
            @"(?>
                <hr>                             |
                <br>                             |
                <!--.*?-->                       |
                <[^/>]*/>                        |
                (?<opentag><(?!/)[^/>]*>)        |
                (?<closetag-opentag></[^/>]*>)   |
                [^<>]*)*
            (?(opentag)(?!))";

        private static class Group
        {
            public const string Name = "name";
            public const string Value = "value";
        }

        private static readonly string HiddenInputPatern =
            $@"<input(?:\s*type=""hidden""|\s*name=""(?<{Group.Name}>[^""]*)""|\s*value=""(?<{Group.Value}>[^""]*)""){{3}}\s*/>";

        private static readonly Regex HiddenInputRegex = new Regex(HiddenInputPatern, RegexOptions.Compiled);

        public static Tuple<string, string> ParseHiddenInput(Capture capture)
        {
            Match match = HiddenInputRegex.Match(capture.Value);
            if (match.Success)
            {
                return new Tuple<string, string>(match.Groups[Group.Name].Value, match.Groups[Group.Value].Value);
            }
            return null;
        }
    }
}
