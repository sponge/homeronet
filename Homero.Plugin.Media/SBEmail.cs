using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public List<string> RegisteredTextCommands { get; } = new List<string> { "sbemail" };

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            e.ReplyTarget.Send($"http://www.homestarrunner.com/sbemail{new Random().Next(206)}.html");
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}