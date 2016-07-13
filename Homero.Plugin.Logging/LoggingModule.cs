using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Logging
{
    public class LoggingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Log>().InSingletonScope();
        }
    }
}