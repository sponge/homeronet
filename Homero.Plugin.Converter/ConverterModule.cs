using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace Homero.Plugin.Converter
{
    public class ConverterModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<Beats>().InSingletonScope();
            Bind<IPlugin>().To<Currency>().InSingletonScope();
            Bind<IPlugin>().To<Temperature>().InSingletonScope();
            Bind<IPlugin>().To<Saturn>().InSingletonScope();
            Bind<IPlugin>().To<Jab>().InSingletonScope();
        }
    }
}
