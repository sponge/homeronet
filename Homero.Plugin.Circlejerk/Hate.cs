using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;
using Homero.Utility;

namespace Homero.Plugin.Circlejerk {
    public class Hate : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "lilpp", "sponge" };

        private Random _random;

        public Hate(IMessageBroker broker) {
            _random = new Random();

            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            var lines = File.ReadAllLines(Path.Combine(Paths.ResourceDirectory, $"{e.Command.Command}_list.txt"));

            var i = _random.Next(lines.Length);

            if (e.Command.Command == "lilpp") {
                client?.ReplyTo(e.Command, $"<LilPP> i hate {lines[i]}");
            }
            else if (e.Command.Command == "sponge") {
                var verb = i % 3 == 0 ? "love" : "hate";
                client?.ReplyTo(e.Command, $"<sponge> i {verb} {lines[i]}");
            }
            
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}