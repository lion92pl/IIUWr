using System;
using System.Collections.Generic;

namespace IIUWr.Fereol.Model
{
    public class Course
    {
        //TODO implement

        public int Id { get; set; }

        //TODO change to URL if possible
        public string URL { get; set; }

        /// <summary>Semester in which this course taken place</summary>
        public Semester Semester { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool? CanEnroll { get; set; }

        public DateTimeOffset? CanEnrollFrom { get; set; }

        public int? ECTS { get; set; }

        public bool? Exam { get; set; }

        public bool? English { get; set; }

        //TODO hours per tutorial type
        //TODO grupy efektów kształcenia (EffectTypes), tagi, percentage?

        public IEnumerable<Tutorial> Tutorials { get; set; }
    }
}
