using IIUWr.Fereol.Interface;
using LionCub.Patterns.DependencyInjection;
using HTMLParsing = IIUWr.Fereol.HTMLParsing;

namespace IIUWr
{
    public static class ConfigureIoC
    {
        public static class Fereol
        {
            public static void HTMLParsing()
            {
                IoC.AsSingleton<IConnection, HTMLParsing.Connection>();
                IoC.AsSingleton<HTMLParsing.Interface.IConnection, HTMLParsing.Connection>();
                IoC.AsSingleton<ICoursesService, HTMLParsing.CoursesService>();
            }
        }
    }
}
