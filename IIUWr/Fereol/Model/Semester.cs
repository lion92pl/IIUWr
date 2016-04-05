using IIUWr.Fereol.Model.Enums;
using System.Collections.Generic;

namespace IIUWr.Fereol.Model
{
    public class Semester
    {
        //TODO implement
        public string Year { get; set; }
        public YearHalf YearHalf { get; set; }
        public int Id { get; set; }

        public IEnumerable<Course> Courses { get; set; }

        public override string ToString()
        {
            return $"{Year} {YearHalf}";
        }
    }
}
