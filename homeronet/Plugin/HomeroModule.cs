using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Plugin {

    public class HomeroModule : NinjectModule {

        public override void Load() {
            // TODO: Define per-client scopes if requested.
            Bind<IPlugin>().To<Homero>().InSingletonScope();
            Bind<IPlugin>().To<Tone>().InSingletonScope();
            Bind<IPlugin>().To<DiscordNowPlaying>().InSingletonScope();
            Bind<IPlugin>().To<Fortune>().InSingletonScope();
            Bind<IPlugin>().To<SieveTest>().InSingletonScope();

        }
    }
}