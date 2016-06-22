﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace Homero.Plugin.Goon
{
    public class GoonModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Fortune>().InSingletonScope();
            Bind<IPlugin>().To<Border>().InSingletonScope();
            Bind<IPlugin>().To<Thinker>().InSingletonScope();
            Bind<IPlugin>().To<Trek>().InSingletonScope();
        }
    }
}