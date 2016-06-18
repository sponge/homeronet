using homeronet.Client;
using homeronet.EventArgs;
using homeronet.Messages;
using homeronet.Plugin;
using homeronet.Properties;
using Newtonsoft.Json;
using Ninject;
using Ninject.Parameters;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ninject.Activation;

namespace homeronet
{
    public class Program
    {
        public static IKernel Kernel { get; private set; }
        public static Logger Log { get; private set; }
        public static AuthenticationConfigurationRoot AuthConfigRoot { get; private set; }

        private static void Main(string[] args)
        {
            Log = LogManager.GetLogger("Homero");
            Log.Info("Homero.NET - Startup");

            Log.Debug("Building kernel");
            Kernel = new StandardKernel(new HomeroModule());

            Log.Info("Reading client authentication configuration file..");
            AuthConfigRoot = new AuthenticationConfigurationRoot();
            AuthConfigRoot.Clients = JsonConvert.DeserializeObject<Dictionary<string, ClientAuthenticationConfiguration>> (File.ReadAllText(@"clientauth.json"));

            if (AuthConfigRoot.Clients == null)
            {
                Log.Error("Configuration root could not load.");
                Console.ReadKey();
            }

            Log.Debug("Building configuration factory");
            Kernel.Bind<IClientAuthenticationConfiguration>().ToMethod(GetAuthConfiguration);

            Log.Debug("Binding Discord");
            Kernel.Bind<IClient>().To<DiscordClient>().InSingletonScope().WithParameter(new Parameter("ClientName", "DiscordClient", true));

            Log.Info("Loading all plugins.");
            foreach (IPlugin plugin in Kernel.GetAll<IPlugin>())
            {
                try
                {
                    Log.Info($"Starting ${plugin.GetType()}");
                    plugin.Startup();
                }
                catch (Exception e)
                {
                    Log.Warn($"Error starting {plugin.GetType()}.");
                    Log.Error(e);
                }
            }

            foreach (IClient client in Kernel.GetAll<IClient>())
            {
                Log.Info($"Connecting {client.GetType()}.");
                try
                {
                    client.Connect();
                    Log.Info($"{client.GetType()} connected.");
                    client.MessageReceived += ClientOnMessageReceived;
                }
                catch (Exception e)
                {
                    Log.Warn($"Error connecting {client.GetType()}.");
                    Log.Error(e);
                }
            }

            Log.Info("Homero running. Press any key to quit.");
            Console.ReadKey();
        }

        private static ClientAuthenticationConfiguration GetAuthConfiguration(IContext context)
        {
            // TODO: Proper config factory.
            IParameter clientNameParam = context.Parameters.FirstOrDefault(x => x.Name == "ClientName");
            if (clientNameParam != null)
            {
                string clientName = clientNameParam.GetValue(context, null) as string;
                if (!String.IsNullOrEmpty(clientName) && AuthConfigRoot.Clients.ContainsKey(clientName))
                {
                    return AuthConfigRoot.Clients[clientName];
                }
                else
                {
                    Log.Warn("A client asked for auth details without a client name! Returning empty config.");
                    return new ClientAuthenticationConfiguration();
                }
            }
            return null;
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