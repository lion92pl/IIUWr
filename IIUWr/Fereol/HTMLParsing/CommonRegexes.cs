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
    }
}
