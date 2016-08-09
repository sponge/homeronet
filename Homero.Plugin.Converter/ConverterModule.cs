using Ninject.Modules;

namespace Homero.Plugin.Converter
{
    public class ConverterModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Beats>().InSingletonScope();
            Bind<IPlugin>().To<Currency>().InSingletonScope();
            Bind<IPlugin>().To<Temperature>().InSingletonScope();
            Bind<IPlugin>().To<Saturn>().InSingletonScope();
            Bind<IPlugin>().To<Jab>().InSingletonScope();
            Bind<IPlugin>().To<CodeEval>().InSingletonScope();
            Bind<IPlugin>().To<Bitcoin>().InSingletonScope();
        }
    }
}