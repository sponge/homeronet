using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Goon {
    public class Bully : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "bully" };

        public Bully(IMessageBroker broker) {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            var outStr = "";

            if (e.Command.Arguments.Count > 0) {
                outStr += string.Join(" ", e.Command.Arguments) + ": ";
            }

            if (sender is IrcClient) {
                outStr += "I feel offended by your recent action(s). Please read http://stop-irc-bullying.eu/stop";
            } else {
                outStr += "I feel offended by your recent action(s). Even though this isn't IRC, please read http://stop-irc-bullying.eu/stop";
            }

            client?.ReplyTo(e.Command, outStr);
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}