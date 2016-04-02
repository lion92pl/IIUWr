using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;

namespace IIUWr.Fereol.HTMLParsing
{
    public class Courses
    {
        private static readonly Uri Endpoint = new Uri(@"https://zapisy.ii.uni.wroc.pl/");
        private const string CoursesPath = @"courses/";

        private const string CourseListPattern = @"(?=<div\s+id=""course-list""[^>]*>)";
        private const string SemesterPattern = @"(?<semester><div\s+class=""semester""[^>]*>)";

        private const string TagsPattern = @"(?><!--.*?-->|<[^>]*/>|(?<opentag><(?!/)[^>]*[^/]>)|(?<-opentag></[^>]*[^/]>)|[^<>]*)*(?(opentag)(?!))";

        public async void GetData()
        {
            using (var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter())
            {
                filter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);

                using (Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient(filter))
                {
                    Uri uri = new Uri(Endpoint, CoursesPath);
                    var page = await client.GetStringAsync(uri);

                    Regex listRegex = new Regex(CourseListPattern + TagsPattern);
                    var list = listRegex.Match(page);

                    Regex semesterRegex = new Regex(SemesterPattern + TagsPattern);
                    var semesters = semesterRegex.Matches(list.Value);
                }
            }
        }
    }
}
