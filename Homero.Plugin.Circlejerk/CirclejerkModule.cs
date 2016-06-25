using Ninject.Modules;

namespace Homero.Plugin.Circlejerk
{
    public class CirclejerkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Pepito>().InSingletonScope();
            Bind<IPlugin>().To<Reddit>().InSingletonScope();
            Bind<IPlugin>().To<Tone>().InSingletonScope();
            Bind<IPlugin>().To<YeahWoo>().InSingletonScope();
            Bind<IPlugin>().To<Depths>().InSingletonScope();
            Bind<IPlugin>().To<Clump>().InSingletonScope();
            Bind<IPlugin>().To<Hate>().InSingletonScope();
        }
    }
}