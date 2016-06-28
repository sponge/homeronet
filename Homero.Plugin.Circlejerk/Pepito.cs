using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;

namespace Homero.Plugin.Circlejerk
{
    public class Pepito : IPlugin
    {
        public Pepito(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> {"pepito"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;
            var amt = new Random().Next(68, 421);
            var hooray = amt == 100 ? "💯" : amt.ToString();
            client?.ReplyTo(e.Command, $"<peptio> hey guys i just ate {hooray} pills");
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}