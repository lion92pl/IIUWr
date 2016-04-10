using IIUWr.Fereol.Interface;
using LionCub.Patterns.DependencyInjection;
using HTMLParsing = IIUWr.Fereol.HTMLParsing;

namespace IIUWr
{
    public static class ConfigureIoC
    {
        public static void All()
        {
            ViewModels();
            Fereol.HTMLParsing();
        }

        public static void ViewModels()
        {
            IoC.AsSingleton<MainPage.ViewModel>();
        }

        public static class Fereol
        {
            public static void HTMLParsing()
            {
                IoC.AsSingleton<IConnection, HTMLParsing.Connection>();
                IoC.AsSingleton<HTMLParsing.Interface.IConnection, HTMLParsing.Connection>();
                IoC.AsSingleton<ICredentialsManager, HTMLParsing.CredentialsManager>();
                IoC.AsSingleton<HTMLParsing.Interface.ISessionManager, HTMLParsing.CredentialsManager>();

                IoC.AsSingleton<ICoursesService, HTMLParsing.CoursesService>();
            }
        }
    }
}
