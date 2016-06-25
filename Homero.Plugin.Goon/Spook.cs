using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;

namespace Homero.Plugin.Goon
{
    public class Spook : IPlugin
    {
        public Spook(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> {"spook", "boo"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            var name = e.Command.Arguments.Count > 0 ? string.Join(" ", e.Command.Arguments) : "boo!";

            var boo = @" .-.
(o o) {0}
| O \
 \   \
  `~~~'";

            if (client?.MarkdownSupported == true)
            {
                boo = $"```{boo}```";
            }

            client?.ReplyTo(e.Command, string.Format(boo, name));
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}