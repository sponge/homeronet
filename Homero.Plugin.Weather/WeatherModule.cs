using Ninject.Modules;

namespace Homero.Plugin.Weather
{
    public class WeatherModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Weather>().InSingletonScope();
        }
    }
}