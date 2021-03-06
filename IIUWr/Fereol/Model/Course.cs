﻿using IIUWr.Fereol.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IIUWr.Fereol.Model
{
    public class Course : INotifyPropertyChanged
    {
        public int Id { get; set; }
        
        //TODO move to HTMLParsing as implementation specific (here create interface, same with other types)
        public string Path { get; set; }

        //TODO remove or change to Id only
        /// <summary>Semester in which this course taken place</summary>
        public Semester Semester { get; set; }

        public string Name { get; set; }

        public CourseType Type { get; set; }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

        private bool? _canEnroll;
        public bool? CanEnroll
        {
            get { return _canEnroll; }
            set
            {
                if (_canEnroll != value)
                {
                    _canEnroll = value;
                    PropertyChanged.Notify(this);
                }
            }
        }

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
                    PropertyChanged.Notify(this);
                }
            }
        }

        public bool? Exam { get; set; }

        public bool? English { get; set; }

        public bool? SuggestedFor1Year { get; set; }

        private bool? _wasEnrolled;
        public bool? WasEnrolled
        {
            get { return _wasEnrolled; }
            set
            {
                if (_wasEnrolled != value)
                {
                    _wasEnrolled = value;
                    PropertyChanged.Notify(this);
                }
            }
        }
        
        //TODO hours per tutorial type
        //TODO grupy efektów kształcenia (EffectTypes), tagi, percentage?

        public event PropertyChangedEventHandler PropertyChanged;

        #region Equality

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Id.Equals((obj as Course).Id);
        }

        public override int GetHashCode()
        {
            return Id;
        }
        
        public static bool operator ==(Course x, Course y)
        {
            return x?.Id == y?.Id;
        }

        public static bool operator !=(Course x, Course y)
        {
            return x?.Id != y?.Id;
        }

        #endregion

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
