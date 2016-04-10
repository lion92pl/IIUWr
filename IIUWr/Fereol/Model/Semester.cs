﻿using IIUWr.Fereol.Model.Enums;
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
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return Id.Equals((obj as Semester).Id);
        }
        
        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return $"{Year} {YearHalf}";
        }
    }
}
