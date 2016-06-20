using Ninject.Modules;

namespace Homero.Plugin {

    public class HomeroModule : NinjectModule {

        public override void Load() {
            // TODO: Define per-client scopes if requested.
            Bind<IPlugin>().To<Homero>().InSingletonScope();
            Bind<IPlugin>().To<Tone>().InSingletonScope();
            Bind<IPlugin>().To<DiscordNowPlaying>().InSingletonScope();
            Bind<IPlugin>().To<Fortune>().InSingletonScope();
        }
    }
}