using System;
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
            Bind<IPlugin>().To<Bully>().InSingletonScope();
            Bind<IPlugin>().To<Wip>().InSingletonScope();
            Bind<IPlugin>().To<Aus>().InSingletonScope();
            Bind<IPlugin>().To<Hys>().InSingletonScope();
            Bind<IPlugin>().To<Spook>().InSingletonScope();
            Bind<IPlugin>().To<Dominions>().InSingletonScope();
            Bind<IPlugin>().To<DominionsRandomNumber>().InSingletonScope();
        }
    }
}
