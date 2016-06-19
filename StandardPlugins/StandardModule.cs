using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Plugin;
using Ninject.Modules;

namespace Homeronet.Plugin.Standard
{
    public class StandardModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Ping>();
        }
    }
}
