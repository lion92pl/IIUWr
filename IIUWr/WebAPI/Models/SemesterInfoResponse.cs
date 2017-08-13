using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IIUWr.Fereol.WebAPI.Models
{
    internal class SemesterInfoResponse
    {
        [JsonProperty("courseList")]
        public List<Course> Courses { get; set; }
    }

    internal class Course
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("exam")]
        public bool Exam { get; set; }

        [JsonProperty("suggested_for_first_year")]
        public bool SuggestedForFirstYear { get; set; }

        [JsonProperty("is_recording_open")]
        public bool RecordingOpen { get; set; }

        [JsonProperty("english")]
        public bool English { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("teacher")]
        public int TeacherId { get; set; }

        [JsonProperty("tags")]
        public int[] Tags { get; set; }

        [JsonProperty("effects")]
        public int[] Effects { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        public static implicit operator IIUWr.Fereol.Model.Course(Course course)
        {
            return new IIUWr.Fereol.Model.Course()
            {
                Name = course.Name,
                English = course.English,
                Exam = course.Exam,
                Id = course.Id,
                SuggestedFor1Year = course.SuggestedForFirstYear,
                Path = course.Url,
                Type = IIUWr.Fereol.Model.CourseType.Find(course.Type)
            };
        }
    }
}
