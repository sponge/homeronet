using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.plugins;
using Ninject.Modules;

namespace homeronet.Plugins
{
    public class HomeroModule : NinjectModule
    {
        public override void Load()
        {
            // TODO: Define per-client scopes if requested.
            Kernel.Bind<IPlugin>().To<Homero>().InSingletonScope();
        }
    }
}
