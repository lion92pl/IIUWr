using IIUWr.Fereol.Common;
using IIUWr.Fereol.Interface;
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
#if DEBUG
            IoC.AsInstance(new Uri(@"http://192.168.1.150:8002/"));
#else
            IoC.AsInstance(new Uri(@"https://zapisy.ii.uni.wroc.pl/"));
#endif

            ViewModels();
            Fereol.Common();

            Fereol.HTMLParsing();
        }

        public static void ViewModels()
        {
            IoC.AsSingleton<SemestersViewModel>();
            IoC.PerRequest<SemesterViewModel>();
            IoC.PerRequest<CourseViewModel>();
            IoC.PerRequest<TutorialViewModel>();
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
