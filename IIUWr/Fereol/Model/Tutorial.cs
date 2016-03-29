using IIUWr.Fereol.Model.Enums;
using System.Collections.Generic;

namespace IIUWr.Fereol.Model
{
    public class Tutorial
    {
        //TODO TimeAndLocation + Enrollment ???

        //TODO implement
        public Course Course { get; set; }

        public TutorialTypeEnum Type { get; set; }

        public IEnumerable<TimeAndLocation> TimesAndLocations { get; set; }

        //TODO english name?
        public Employee Prowadzący { get; set; }

        //TODO right spelling?
        #region Enrollment

        public int Limit { get; set; }

        public int Enrolled { get; set; }

        public int Queue { get; set; }

        public IEnumerable<string> Students { get; set; }

        #endregion
    }
}
