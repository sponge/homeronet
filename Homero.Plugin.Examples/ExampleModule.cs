using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace Homero.Plugin.Examples
{
    /// <summary>
    /// An example module that holds all plugin instances and injects them in to the kernel.
    /// </summary>
    /// <seealso cref="Ninject.Modules.NinjectModule" />
    public class ExampleModule : NinjectModule 
    {
        /// <summary>
        /// Executed when the module is loading into the kernel. This is where you bind your plugin instances.
        /// </summary>
        public override void Load()
        {
            Bind<IPlugin>().To<StoreTest>().InSingletonScope();
            Bind<IPlugin>().To<LastMessage>().InSingletonScope();
        }
    }
}
