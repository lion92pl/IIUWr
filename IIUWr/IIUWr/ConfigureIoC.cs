using IIUWr.Fereol.Common;
using IIUWr.Fereol.Interface;
using IIUWr.ViewModelInterfaces.Fereol;
using IIUWr.ViewModels.Fereol;
using LionCub.Patterns.DependencyInjection;
using System;
using HTMLParsing = IIUWr.Fereol.HTMLParsing;

namespace IIUWr
{
    public static class ConfigureIoC
    {
        public static void All()
        {
            IoC.AsInstance(new Uri(@"https://zapisy.ii.uni.wroc.pl/"));

            ViewModels();
            Fereol.Common();
            Fereol.HTMLParsing();
        }

        public static void ViewModels()
        {
            IoC.AsSingleton<ISemestersViewModel, SemestersViewModel>();
            IoC.PerRequest<ISemesterViewModel, SemesterViewModel>();
            IoC.PerRequest<ICourseViewModel, CourseViewModel>();
        }

        public static class Fereol
        {
            public static void Common()
            {
                IoC.AsSingleton<ICredentialsManager, CredentialsManager>();
                IoC.AsSingleton<ISessionManager, CredentialsManager>();
            }

            public static void HTMLParsing()
            {
                IoC.AsSingleton<IConnection, HTMLParsing.Connection>();
                IoC.AsSingleton<HTMLParsing.Interface.IHTTPConnection, HTMLParsing.Connection>();

                IoC.AsSingleton<ICoursesService, HTMLParsing.CoursesService>();
            }
        }
    }
}
