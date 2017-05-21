using IIUWr.Fereol.Model;
using LionCub.Patterns.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIUWr.ViewModels.Fereol
{
    public class FiltersViewModel
    {
        public List<CourseTypeFilterViewModel> CourseTypeFilters { get; private set; }

        public Func<Course, bool> CurrentFilter
        {
            get
            {
                return course => CourseTypeFilters.Single(f => f.Type == course.Type).Selected;
            }
        }

        public FiltersViewModel()
        {
            CreateCourseTypeFilters();
        }

        private void CreateCourseTypeFilters()
        {
            var filters = new List<CourseTypeFilterViewModel>();
            foreach (var type in CourseType.Types)
            {
                var vm = IoC.Get<CourseTypeFilterViewModel>();
                vm.Type = type;
                filters.Add(vm);
                CreateChildCourseTypeFilters(vm, filters);
            }

            CourseTypeFilters = filters;
        }

        private void CreateChildCourseTypeFilters(CourseTypeFilterViewModel filter, List<CourseTypeFilterViewModel> filters)
        {
            if (filter.Type.Children == null)
            {
                return;
            }

            filter.Children = new List<CourseTypeFilterViewModel>();

            foreach (var type in filter.Type.Children)
            {
                var vm = IoC.Get<CourseTypeFilterViewModel>();
                vm.Type = type;
                vm.Parent = filter;
                filter.Children.Add(vm);
                filters.Add(vm);
                CreateChildCourseTypeFilters(vm, filters);
            }
        }
    }
}
