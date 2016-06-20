using System;
using System.Collections.Generic;
using Homero.Client;
using Homero.EventArgs;
using Homero.Services;

namespace Homero.Plugin.Converter
{
    public class Beats : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() { "beats" };

        public Beats(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;

            DateTime now = DateTime.UtcNow;
            double beatsTime = Math.Floor((now.Second + (now.Minute * 60) + (now.Hour * 3600)) / 86.4f);
            client?.ReplyTo(e.Command, $"utc time: {now.ToString("H:mm:ss")} | beat time: @{beatsTime}");

        }

    }
}
