using homeronet.Client;
using homeronet.EventArgs;
using homeronet.Messages;
using homeronet.Plugin;
using homeronet.Properties;
using Newtonsoft.Json;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using homeronet.Manager.Assembly;
using homeronet.Services;
using Ninject.Activation;
using Ninject.Extensions.Conventions;

namespace homeronet
{
    public class Program
    {
        public static IKernel Kernel { get; private set; }

        private static ILogger Logger { get; set; }
        private static FileSystemWatcher PluginWatcher { get; set; }

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetShadowCopyFiles();

            Kernel = new StandardKernel();
            Kernel.Bind<ILogger>().ToMethod(context => LoggerFactory.Instance.GetLogger(context.Request?.Target?.Member?.DeclaringType?.Name));
            Logger = Kernel.Get<ILogger>();
            Logger.Info("Logger loaded.");

            Logger.Debug("Setting up Config factory");
            Kernel.Bind<IConfiguration>().ToMethod(context => ConfigurationFactory.Instance.GetConfiguration(context.Request?.Target?.Member?.DeclaringType?.Name));
            
            Logger.Info("Loading standard plugins.");
            Kernel.Load(new HomeroModule());

            Logger.Info("Scanning and loading plugin directory.");
            Kernel.Load("Plugins\\Homeronet.Plugin.*.dll");
            //PluginAppDomain pluginDomain = new PluginAppDomain(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Plugins","Homeronet.Plugin.Standard.dll"), Kernel);
            
            Logger.Info("Setting up file change monitoring.");
            PluginWatcher = new FileSystemWatcher();

            // TODO: Configurable Path, don't depend on appdomain.
            PluginWatcher.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\");

            PluginWatcher.NotifyFilter = NotifyFilters.Attributes |
    NotifyFilters.CreationTime |
    NotifyFilters.FileName |
    NotifyFilters.LastAccess |
    NotifyFilters.LastWrite |
    NotifyFilters.Size |
    NotifyFilters.Security;
            PluginWatcher.Filter = "Homeronet.Plugin.*.dll";
            PluginWatcher.Changed += PluginWatcherOnChanged;
            PluginWatcher.Created += PluginWatcherOnChanged;
            PluginWatcher.Renamed += PluginWatcherOnChanged;

            PluginWatcher.EnableRaisingEvents = true;


            Logger.Debug("Binding Discord");
            Kernel.Bind<IClient>().To<DiscordClient>().InSingletonScope();

            Logger.Info("Loading all plugins.");
            foreach (IPlugin plugin in Kernel.GetAll<IPlugin>())
            {
                try
                {
                    Logger.Info($"Starting ${plugin.GetType()}");
                    plugin.Startup();
                }
                catch (Exception e)
                {
                    Logger.Warn($"Error starting {plugin.GetType()}.");
                    Logger.Error(e);
                }
            }

            foreach (IClient client in Kernel.GetAll<IClient>())
            {
                Logger.Info($"Connecting {client.GetType()}.");
                try
                {
                    client.Connect();
                    Logger.Info($"{client.GetType()} connected.");
                    client.MessageReceived += ClientOnMessageReceived;
                }
                catch (Exception e)
                {
                    Logger.Warn($"Error connecting {client.GetType()}.");
                    Logger.Error(e);
                }
            }

            Logger.Info("Homero running. Press any key to quit.");
            Console.ReadKey();
        }

        private static void PluginWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Logger.Info("Plugin folder DLL change detected. Reloading all plugins.");
            Kernel.Unbind<IPlugin>();
            Kernel.Unload("Plugins\\Homeronet.Plugin.*.dll");
            Kernel.Load("Plugins\\Homeronet.Plugin.*.dll");
        }

        private static void ClientOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Is this a command?
            if (e.Message.Message.StartsWith(Settings.Default.CommandPrefix))
            {
                // TODO: TextCommand constructor to auto-parse message.
                TextCommand command = new TextCommand();
                command.InnerMessage = e.Message;
                string[] splitMsg = e.Message.Message.Split(' ');
                command.Command = splitMsg[0].TrimStart(Settings.Default.CommandPrefix.ToCharArray());
                if (splitMsg.Length > 1)
                {
                    command.Arguments = splitMsg.Skip(1).ToList();
                }

                // Dispatch to all applicable plugins.

                // TODO: Reduce this to one kernel iteration for standard and plugin dispatch.
                foreach (IPlugin plugin in Kernel.GetAll<IPlugin>())
                {
                    if (plugin.RegisteredTextCommands?.Contains(command.Command) == true)
                    {
                        // Check the implementation below for more info on how this works.
                        Task<IStandardMessage> commandTask = plugin.ProcessTextCommand(command);
                        if (commandTask != null)
                        {
                            commandTask.ContinueWith(
                                delegate (Task task, object o)
                                {
                                    IClient client = o as IClient;
                                    if (client != null)
                                    {
                                        Task<IStandardMessage> castTask = task as Task<IStandardMessage>;
                                        if (castTask?.Result != null)
                                        {
                                            client.SendMessage(castTask.Result);
                                        }
                                    }
                                }, sender);
                            commandTask.Start();
                        }
                    }
                }
            }

            // Dispatch to all plugins we can.
            foreach (IPlugin plugin in Kernel.GetAll<IPlugin>())
            {
                /* Standard Text distribution! */

                // Get the new task.
                Task<IStandardMessage> processTask = plugin.ProcessTextMessage(e.Message);

                // Does the plugin even bother parsing?
                if (processTask != null)
                {
                    // Setup callback handling
                    processTask.ContinueWith(
                        delegate (Task task, object o)
                        {
                            IClient client = o as IClient;
                            if (client != null)
                            {
                                Task<IStandardMessage> castTask = task as Task<IStandardMessage>;
                                if (castTask?.Result != null)
                                {
                                    client.SendMessage(castTask.Result);
                                }
                            }
                        }, sender);
                    // Fire the root task!
                    processTask.Start();
                }
            }
        }
    }
}