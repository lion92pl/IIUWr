using IIUWr.Fereol.HTMLParsing;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTests.Fereol.HTMLParsing
{
    [TestClass]
    public class CommonRegexesTests
    {
        [TestCategory(Categories.Parsing)]
        [DataTestMethod]
        [DataRow("NotAuthenticated", false, false)]
        [DataRow("Student", true, true)]
        public void AuthenticationStatusTest(string htmlFileName, bool authenticated, bool student)
        {
            string htmlPage = System.IO.File.ReadAllText($"Fereol/HTMLParsing/Pages/Authentication/{htmlFileName}.html");
            var actual = CommonRegexes.ParseAuthenticationStatus(htmlPage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(authenticated, actual.Authenticated);
            Assert.AreEqual(student, actual.IsStudent);
        }
    }
}
