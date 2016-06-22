using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Circlejerk
{
    public class Pepito : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() {"pepito"};

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

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            var amt = new Random().Next(68, 421);
            string hooray = amt == 100 ? "💯" : amt.ToString();
            client?.ReplyTo(e.Command ,$"<peptio> hey guys i just ate {hooray} pills");
        }

        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}