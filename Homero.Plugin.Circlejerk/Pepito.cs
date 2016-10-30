using Homero.Core.EventArgs;
using Homero.Core.Services;
using System;
using System.Collections.Generic;

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

        public List<string> RegisteredTextCommands { get; } = new List<string> { "pepito" };

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var amt = new Random().Next(68, 421);
            var hooray = amt == 100 ? "💯" : amt.ToString();
            e.ReplyTarget.Send($"<peptio> hey guys i just ate {hooray} pills");
        }
    }
}