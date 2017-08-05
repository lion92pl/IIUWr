using IIUWr.Fereol.Model.Enums;
using System.Collections.Generic;

namespace IIUWr.Fereol.Model
{
    public class Tutorial
    {
        //TODO TimeAndLocation + Enrollment ???

        //TODO implement
        public int Id { get; set; }

        public Course Course { get; set; }

        public TutorialType Type { get; set; }
        
        public Employee Teacher { get; set; }

        public bool AdvancedGroup { get; set; }

        public IList<TimeAndLocation> Terms { get; set; }

        #region Enrollment

        public bool InterdisciplinaryGroup => LimitInterdisciplinary > 0;

        public int LimitInterdisciplinary { get; set; }

        public int Limit { get; set; }

        public int EnrolledInterdisciplinary { get; set; }

        public int Enrolled { get; set; }

        public int Queue { get; set; }

        public IList<string> Students { get; set; }

        public bool IsEnrolled { get; set; }

        public bool IsQueued { get; set; }

        public int Priority { get; set; }

        #endregion
    }
}
