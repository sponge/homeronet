using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;

namespace Homero.Plugin.Media
{
    public class SbEmail : IPlugin
    {
        public SbEmail(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }


        public List<string> RegisteredTextCommands { get; } = new List<string> {"sbemail"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;
            client?.ReplyTo(e.Command, $"http://www.homestarrunner.com/sbemail{new Random().Next(206)}.html");
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}