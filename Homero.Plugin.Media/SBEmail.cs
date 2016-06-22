using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Media
{
    public class SbEmail : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() {"sbemail"};

        public SbEmail(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            client?.ReplyTo(e.Command, $"http://www.homestarrunner.com/sbemail{new Random().Next(206)}.html");
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

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}