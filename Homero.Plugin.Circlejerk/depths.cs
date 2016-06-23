using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Circlejerk {
    public class Depths : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "depths" };

        public Depths(IMessageBroker broker) {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            var str = String.Join(" ", e.Command.Arguments).Replace('o', 'ø');
            client?.ReplyTo(e.Command, str);
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}