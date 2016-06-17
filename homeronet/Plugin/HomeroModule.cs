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
            Kernel.Bind<IPlugin>().To<Homero>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Tone>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<DiscordNowPlaying>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Fortune>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Trek>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Weather>().InSingletonScope();
        }
    }
}