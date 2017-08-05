using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Model
{
    public class TimeAndLocation
    {
        public DayOfWeek Day { get; set; }

        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public string Location { get; set; }
    }
}
