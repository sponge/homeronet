using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.Services;
using Homero.Core.Utility;
using Homero.Plugin;
using Ninject;
using Ninject.Extensions.Conventions;
using Topshelf;

namespace Homero.Client
{
    // The heart of things.
    public class Heart : ServiceControl
    {
        public IKernel Kernel { get; private set; }
        public ILogger Logger { get; private set; }

        public bool Start(HostControl hostControl)
        {
            Kernel = new StandardKernel();
            Kernel.Bind<ILogger>()
                .ToMethod(
                    context => LoggerFactory.Instance.GetLogger(context.Request?.Target?.Member?.DeclaringType?.Name));
            Logger = Kernel.Get<ILogger>();
            Logger.Info("Logger loaded.");

            Logger.Debug("Setting up Config factory");
            Kernel.Bind<IConfiguration>()
                .ToMethod(
                    context =>
                        JsonConfigurationFactory.Instance.GetConfiguration(
                            context.Request?.Target?.Member?.DeclaringType?.Name));

            Logger.Debug("Configuring Message Broker");
            Kernel.Bind<IMessageBroker>().To<MessageBrokerService>().InSingletonScope();

            Logger.Debug("Configuring KV store");
            Kernel.Bind<IStore>().ToMethod(context => KvStoreFactory.Instance.GetKvStore(context.Request?.Target?.Member?.DeclaringType?.Name));

            Logger.Info("Scanning and loading plugin directory.");
            Kernel.Load("Plugins\\Homero.Plugin.*.dll");

            Logger.Debug("Loading all clients.");

            // CHECK: Will this get Core?
            Kernel.Bind(
                x =>
                    x.FromAssembliesMatching("Homero.Core.*")
                        .SelectAllClasses()
                        .InheritedFrom<IClient>()
                        .BindAllInterfaces()
                        .Configure(b => b.InSingletonScope()));

            Logger.Info("Loading all plugins.");
            foreach (var plugin in Kernel.GetAll<IPlugin>())
            {
                try
                {
                    Logger.Info($"Starting {plugin.GetType()}");
                    plugin.Startup();
                }
                catch (Exception e)
                {
                    Logger.Warn($"Error starting {plugin.GetType()}.");
                    Logger.Error(e);
                    Kernel.Unbind(plugin.GetType());
                }
            }

            foreach (var client in Kernel.GetAll<IClient>())
            {
                Logger.Info($"Connecting {client.GetType()}.");
                try
                {
                    client.Connect();
                    Logger.Info($"{client.GetType()} connected.");
                }
                catch (Exception e)
                {
                    Logger.Warn($"Error connecting {client.GetType()}.");
                    Logger.Error(e);
                }
            }

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            // SHUT IT DOWN WE OUT
            Kernel.Dispose();
            return true;
        }
    }
}
