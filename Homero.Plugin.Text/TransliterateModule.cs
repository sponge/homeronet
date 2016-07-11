using Ninject.Modules;

namespace Homero.Plugin.Text
{
    public class WeatherModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Transliterate>().InSingletonScope();
        }
    }
}