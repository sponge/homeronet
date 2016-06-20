using homeronet.Client;
using homeronet.EventArgs;
using homeronet.Messages;
using homeronet.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace homeronet.Plugin
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

        public void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            DateTime now = DateTime.UtcNow;
            string utcTime = now.ToString("H:mm:ss");
            double beatsTime = Math.Floor((now.Second + (now.Minute * 60) + (now.Hour * 3600)) / 86.4f);
            client?.ReplyTo(e.Command, $"utc time: {utcTime} | beat time: @{beatsTime}");
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