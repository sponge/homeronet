using Ninject.Modules;

namespace Homero.Plugin.Pipe
{
    public class PipeModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<PipePlugin>();
        }
    }
}