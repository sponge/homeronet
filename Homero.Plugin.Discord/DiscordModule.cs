using Ninject.Modules;

namespace Homero.Plugin.Discord
{
    public class DiscordModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<NowPlaying>().InSingletonScope();
        }
    }
}