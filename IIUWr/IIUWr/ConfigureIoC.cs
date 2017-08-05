using IIUWr.Fereol.Common;
using IIUWr.Fereol.Interface;
using IIUWr.ViewModels.Fereol;
using LionCub.Patterns.DependencyInjection;
using System;
using Windows.Storage;
using HTMLParsing = IIUWr.Fereol.HTMLParsing;

namespace IIUWr
{
    public static class DemoModeHelper
    {
        private const string Key = "demo-mode";

        public static bool IsDemoMode
        {
            get
            {
                object demoMode;
                ApplicationData.Current.LocalSettings.Values.TryGetValue(Key, out demoMode);
                return demoMode != null && demoMode is bool demo && demo;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values[Key] = value;
            }
        }

        public static bool ToggleModeByLogin(string login)
        {
            if (login == "demo" && !IsDemoMode)
            {
                IsDemoMode = true;
                return true;
            }
            if (login == "notDemo" && IsDemoMode)
            {
                IsDemoMode = false;
                return true;
            }
            return false;
        }
    }

    public static class ConfigureIoC
    {
        public static void All()
        {
            if (DemoModeHelper.IsDemoMode)
            {
#if DEBUG
                IoC.AsInstance(new Uri(@"http://localhost:8002/"));
#else
                IoC.AsInstance(new Uri(@"http://soltysik.net.pl:8002/"));
#endif

            }
            else
            {
                IoC.AsInstance(new Uri(@"https://zapisy.ii.uni.wroc.pl/"));
            }

            ViewModels();
            Fereol.Common();

            Fereol.HTMLParsing();
            Fereol.WebAPI();
        }

        public static void ViewModels()
        {
            IoC.AsSingleton<SemestersViewModel>();
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
                IoC.AsSingleton<IScheduleService, HTMLParsing.ScheduleService>();
            }

            public static void WebAPI()
            {
                IoC.AsSingleton<WebAPI.Interface.ICoursesService, WebAPI.CoursesService>();
            }
        }
    }
}
