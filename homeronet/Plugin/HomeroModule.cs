﻿using Ninject.Modules;
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
            Kernel.Bind<IPlugin>().To<YouTube>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Reddit>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Currency>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Border>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Thinker>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Beats>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<YeahWoo>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Temperature>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<SBEmail>().InSingletonScope();
            Kernel.Bind<IPlugin>().To<Pepito>().InSingletonScope();
        }
    }
}