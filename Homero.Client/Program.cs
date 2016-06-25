using System;
using System.Linq;
using Homero.Core.Client;
using Homero.Core.Services;
using Homero.Plugin;
using Ninject;
using Ninject.Extensions.Conventions;

namespace Homero.Client
{
    public class Program
    {
        public static IKernel Kernel { get; private set; }

        private static ILogger Logger { get; set; }

        private static void Main(string[] args)
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
                        ConfigurationFactory.Instance.GetConfiguration(
                            context.Request?.Target?.Member?.DeclaringType?.Name));

            Logger.Debug("Configuring Message Broker");
            Kernel.Bind<IMessageBroker>().To<MessageBrokerService>().InSingletonScope();

            Logger.Info("Scanning and loading plugin directory.");
            Kernel.Load("Plugins\\Homero.Plugin.*.dll");

            Logger.Debug("Loading all clients.");
            Kernel.Bind(
                x =>
                    x.FromThisAssembly()
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

            Logger.Info("Homero running. Press any key to quit.");
            Console.ReadKey();

            Kernel.GetAll<IPlugin>().ToList().ForEach(x => x.Shutdown());
        }
    }
}