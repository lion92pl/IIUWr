﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IIUWr.Fereol.Model
{
    public class Course : INotifyPropertyChanged
    {
        //TODO implement

        public int Id { get; set; }

        //TODO change to URL if possible
        public string Path { get; set; }

        /// <summary>Semester in which this course taken place</summary>
        public Semester Semester { get; set; }

        public string Name { get; set; }

        public CourseType Type { get; set; }

        public string Description { get; set; }
        
        public bool? CanEnroll { get; set; }

        public DateTimeOffset? CanEnrollFrom { get; set; }

        private int? _ECTS;
        public int? ECTS
        {
            get { return _ECTS; }
            set
            {
                if (_ECTS != value)
                {
                    _ECTS = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ECTS)));
                }
            }
        }

        public bool? Exam { get; set; }

        public bool? English { get; set; }

        public bool? SuggestedFor1Year { get; set; }

        public bool? WasEnrolled { get; set; }

        //TODO hours per tutorial type
        //TODO grupy efektów kształcenia (EffectTypes), tagi, percentage?

        public IEnumerable<Tutorial> Tutorials { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
