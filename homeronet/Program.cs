using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using homeronet.Client;
using homeronet.EventArgs;
using homeronet.Messages;
using homeronet.Plugins;
using homeronet.Properties;
using Newtonsoft.Json;
using Ninject;
using Ninject.Parameters;
using NLog;

namespace homeronet {

    public class Program
    {
        public static IKernel Kernel { get; private set; }
        public static Logger Log { get; private set; }
        private static void Main(string[] args)
        {
            Log = LogManager.GetLogger("Homero");
            Log.Info("Homero.NET - Startup");

            Log.Debug("Building kernel");
            Kernel = new StandardKernel(new HomeroModule());

            Log.Debug("Building configuration factory");
            Kernel.Bind<IClientConfiguration>().ToMethod((context =>
            {
                // TODO: Proper config factory.
                IParameter clientNameParam = context.Parameters.FirstOrDefault(x => x.Name == "ClientName");
                if (clientNameParam != null)
                {
                    string clientName = clientNameParam.GetValue(context, null) as string;
                    using (StreamReader r = new StreamReader("config.json"))
                    {
                        // TODO: Strict contract
                        string json = r.ReadToEnd();
                        dynamic jsonConfig = JsonConvert.DeserializeObject<dynamic>(json);
                        return new ClientConfiguration() { ApiKey = jsonConfig[clientName]["ApiKey"].ToString(), Username = jsonConfig[clientName]["Username"].ToString(), Password = jsonConfig[clientName]["Password"].ToString() };
                    }
                }
                return null;
            }));

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