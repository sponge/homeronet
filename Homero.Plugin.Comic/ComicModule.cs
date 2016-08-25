using Ninject.Modules;

namespace Homero.Plugin.Comic
{
    public class ComicModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<ComicCommand>().InSingletonScope();
        }
    }
}
