using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace Homero.Plugin.Discord
{
    public class DiscordModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPlugin>().To<NowPlaying>().InSingletonScope();
        }
    }
}
