﻿using IIUWr.Fereol.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace IIUWr.Fereol.Interface
{
    public interface ICoursesService
    {
        Task<IEnumerable<Semester>> GetSemesters();
        Task<IEnumerable<Course>> GetCourses(Semester semester);
        Task<bool> FillCourseDetails(Course course);
        Task<IEnumerable<Tutorial>> GetTutorials(Course course);

        Task<bool> Enroll(Tutorial tutorial, bool enroll);
        Task<bool> SetPriority(Tutorial tutorial, int priority);
    }
}
