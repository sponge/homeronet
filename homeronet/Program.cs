using DiscordSharp;
using homeronet.plugins;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace homeronet {

    internal class Program {

        private static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Please specify your bot token on the commandline.");
                Environment.Exit(1);
            }

            List<IPlugin> plugins = new List<IPlugin>();
            Dictionary<string, IPlugin> commandTriggers = new Dictionary<string, IPlugin>();

            DiscordClient client = new DiscordClient(args[0], true);

            var assembly = Assembly.GetExecutingAssembly();
            foreach (string pluginName in Properties.Settings.Default.PluginList) {
                var pluginType = assembly.GetTypes().First(t => t.Name == pluginName);
                IPlugin pluginInstance = (IPlugin)Activator.CreateInstance(pluginType);
                pluginInstance.Startup(client);
                plugins.Add(pluginInstance);

                var trigger = pluginInstance.GetCommandTrigger();
                if (trigger.Length > 0) {
                    commandTriggers.Add(Properties.Settings.Default.CommandPrefix + trigger, pluginInstance);
                }
            }

            client.Connected += (sender, e) => {
                Console.WriteLine($"Connected! User: {e.User.Username}");
            };

            client.MessageReceived += (sender, e) => {
                if (e.MessageText.StartsWith(Properties.Settings.Default.CommandPrefix)) {
                    var parts = e.MessageText.Split(null);
                    if (commandTriggers.ContainsKey(parts[0])) {
                        commandTriggers[parts[0]].CommandTrigger(e);
                    }
                }
            };

            try {
                client.SendLoginRequest();
                client.Connect();
            }
            catch (Exception e) {
                Console.WriteLine("Something went wrong!\n" + e.Message + "\nPress any key to close this window.");
            }

            Console.ReadKey();

            foreach (var plugin in plugins) {
                plugin.Shutdown();
            }

            Environment.Exit(0);
        }
    }
}