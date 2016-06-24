using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Goon {
    public class Spook : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "spook", "boo" };

        public Spook(IMessageBroker broker) {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            var name = e.Command.Arguments.Count > 0 ? string.Join(" ", e.Command.Arguments) : "boo!";

            var boo = @" .-.
(o o) {0}
| O \
 \   \
  `~~~'";

            if (client?.MarkdownSupported == true) {
                boo = $"```{boo}```";
            }

            client?.ReplyTo(e.Command, String.Format(boo, name));
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}