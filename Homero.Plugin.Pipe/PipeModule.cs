﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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