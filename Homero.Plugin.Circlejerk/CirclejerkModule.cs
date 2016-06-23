using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace Homero.Plugin.Circlejerk
{
    public class CirclejerkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Pepito>().InSingletonScope();
            Bind<IPlugin>().To<Reddit>().InSingletonScope();
            Bind<IPlugin>().To<Tone>().InSingletonScope();
            Bind<IPlugin>().To<YeahWoo>().InSingletonScope();
            Bind<IPlugin>().To<ImageTest>().InSingletonScope();
        }
    }
}
