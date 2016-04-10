using IIUWr.Fereol.Model;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Linq;

namespace UnitTests.Fereol.Model
{
    [TestClass]
    public class CourseTypeTests
    {
        [DataTestMethod]
        [DataRow(1, "Inf")]
        [DataRow(5, "I1")]
        [DataRow(38, "I2T")]
        [DataRow(35, "Inne")]
        public void Find(int id, string nameKey)
        {
            var type = CourseType.Find(id);
            Assert.AreEqual(nameKey, type.NameKey);
        }

        [DataTestMethod]
        [DataRow(1, "1")]
        [DataRow(5, "1->5")]
        [DataRow(38, "1->6->38")]
        public void Path(int id, string expected)
        {
            var path = CourseType.Path(id);
            string actual = string.Join("->", path.Select(t => t.Id));
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(1, -1)]
        [DataRow(5, 1)]
        [DataRow(38, 6)]
        public void Parent(int id, int expected)
        {
            var actual = CourseType.Find(id);
            Assert.AreEqual(expected, actual.Parent?.Id ?? -1);
        }
    }
}
