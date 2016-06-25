using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;

namespace Homero.Plugin.Circlejerk
{
    public class Depths : IPlugin
    {
        public Depths(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> {"depths"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            var str = string.Join(" ", e.Command.Arguments).Replace('o', 'ø');
            client?.ReplyTo(e.Command, str);
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}