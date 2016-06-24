using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace Homero.Plugin.Media
{
    public class MediaModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<SbEmail>().InSingletonScope();
            Bind<IPlugin>().To<YouTube>().InSingletonScope();
            Bind<IPlugin>().To<YouTubeDubber>().InSingletonScope();
        }
    }
}
