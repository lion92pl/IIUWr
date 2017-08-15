using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IIUWr.Fereol.WebAPI.Models
{
    [DataContract]
    internal class SemesterInfoResponse
    {
        [DataMember(Name ="courseList")]
        public List<Course> Courses { get; set; }
    }

    [DataContract]
    internal class Course
    {
        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "exam")]
        public bool Exam { get; set; }

        [DataMember(Name = "suggested_for_first_year")]
        public bool SuggestedForFirstYear { get; set; }

        [DataMember(Name = "is_recording_open")]
        public bool RecordingOpen { get; set; }

        [DataMember(Name = "english")]
        public bool English { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "teacher")]
        public int TeacherId { get; set; }

        [DataMember(Name = "tags")]
        public int[] Tags { get; set; }

        [DataMember(Name = "effects")]
        public int[] Effects { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "short_name")]
        public string ShortName { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        public static implicit operator Model.Course(Course course)
        {
            return new Model.Course()
            {
                Name = course.Name,
                English = course.English,
                Exam = course.Exam,
                Id = course.Id,
                SuggestedFor1Year = course.SuggestedForFirstYear,
                Path = course.Url,
                Type = Model.CourseType.Find(course.Type)
            };
        }
    }
}
