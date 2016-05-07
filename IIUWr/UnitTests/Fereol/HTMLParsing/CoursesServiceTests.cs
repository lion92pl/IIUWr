using IIUWr.Fereol.HTMLParsing;
using IIUWr.Fereol.HTMLParsing.Interface;
using IIUWr.Fereol.Model;
using IIUWr.Utils.Refresh;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Linq;
using UnitTests.Mocks.Fereol.HTMLParsing;
using System;
using IIUWr.Fereol.Model.Enums;

namespace UnitTests.Fereol.HTMLParsing
{
    [TestClass]
    public class CoursesServiceTests
    {
        [TestCategory(Categories.Parsing)]
        [TestMethod]
        public void SemestersTest()
        {
            var coursesService = GetCoursesService("List.html");
            var actual = coursesService.GetSemesters().Result;

            CollectionAssert.AreEqual(new Semester[]
            {
                new Semester { Id = 334, Year = "2015/16", YearHalf = YearHalf.Summer },
                new Semester { Id = 333, Year = "2015/16", YearHalf = YearHalf.Winter },
                new Semester { Id = 332, Year = "2014/15", YearHalf = YearHalf.Summer },
                new Semester { Id = 331, Year = "2014/15", YearHalf = YearHalf.Winter },
                new Semester { Id = 330, Year = "2013/14", YearHalf = YearHalf.Summer },
                new Semester { Id = 329, Year = "2013/14", YearHalf = YearHalf.Winter },
                new Semester { Id = 232, Year = "2012/13", YearHalf = YearHalf.Summer },
                new Semester { Id = 328, Year = "2012/13", YearHalf = YearHalf.Winter },
                new Semester { Id = 239, Year = "2011/12", YearHalf = YearHalf.Summer }
            }, actual.ToList(), new SemesterComparer());
        }
        
        [TestCategory(Categories.Parsing)]
        [TestMethod]
        public void CoursesTest()
        {
            var coursesService = GetCoursesService("List.html");
            var actual = coursesService.
                GetCourses(new Semester { Id = 334, Year = "2015/16", YearHalf = YearHalf.Summer }).
                Result;

            //TODO extend compared courses to at least 10
            Assert.AreEqual(55, actual.Count());
            CollectionAssert.AreEqual(new Course[]
            {
                new Course { Id = 3457, Name = "Algebra", Path = "algebra_1516", Type = CourseType.Find(8), WasEnrolled = false, English = false, Exam = true, SuggestedFor1Year = true }
            }, actual.Take(1).ToList(), new CourseComparer());
        }

        [TestCategory(Categories.PassingCourses)]
        [TestCategory(Categories.Parsing)]
        [TestMethod]
        public void Algebra1516Test()
        {
            var coursesService = GetCoursesService("algebra_1516.html");
            var course = new Course { Id = 3457, Name = "Algebra", Path = "algebra_1516", Type = CourseType.Find(8), WasEnrolled = false, English = false, Exam = true, SuggestedFor1Year = true };
            var actual = coursesService.
                RefreshCourse(course).
                Result;

            Assert.AreEqual(true, actual);
            Assert.AreEqual("Algebra", course.Name);
            //TODO add checking part of description HTML
            Assert.AreEqual(778, course.Description.Length);
            Assert.AreEqual(7, course.ECTS);
        }
        
        [Ignore]
        [TestCategory(Categories.FailingCourses)]
        [TestCategory(Categories.Parsing)]
        [TestMethod]
        public void PracticalCSharp1415Test()
        {
            var coursesService = GetCoursesService("kurs_practical_c_enterpirse_software_development.html");
            var course = new Course
            {
                Id = 3343,
                Name = "Kurs: Practical C# Enterprise Software Development",
                Path = "kurs_practical_c_enterpirse_software_development",
                Type = CourseType.Find(40),
                WasEnrolled = false,
                English = false,
                Exam = false,
                SuggestedFor1Year = false
            };

            var actual = coursesService.RefreshCourse(course).Result;
            
            Assert.AreEqual(true, actual);
            Assert.AreEqual("Kurs: Practical C# Enterprise Software Development", course.Name);
            //provide actual value for this course
            Assert.AreEqual(778, course.Description.Length);
            Assert.AreEqual(4, course.ECTS);
        }

        private static CoursesService GetCoursesService(string path)
        {
            IHTTPConnection connection = new HTTPConnection("Fereol/HTMLParsing/Pages/Courses/" + path);
            return new CoursesService(connection, new RefreshTimesManager());
        }

        #region Comparers

        private class SemesterComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                if (x is Semester && y is Semester)
                {
                    var a = x as Semester;
                    var b = y as Semester;

                    return a.Id == b.Id
                        && a.Year == b.Year
                        && a.YearHalf == b.YearHalf
                        ? 0 : a.Id.CompareTo(b.Id);
                }
                return -1;
            }
        }

        private class CourseComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                if (x is Course && y is Course)
                {
                    var a = x as Course;
                    var b = y as Course;

                    return a.Id == b.Id
                        && a.Name == b.Name
                        && a.Path == b.Path
                        && a.Type == b.Type
                        && a.WasEnrolled == b.WasEnrolled
                        && a.SuggestedFor1Year == b.SuggestedFor1Year
                        && a.Exam == b.Exam
                        && a.English == b.English
                        && a.ECTS == b.ECTS
                        && a.Description == b.Description
                        && a.CanEnroll == b.CanEnroll
                        && a.CanEnrollFrom == b.CanEnrollFrom
                        ? 0 : a.Id.CompareTo(b.Id);
                }
                return -1;
            }
        }

        #endregion

    }
}
