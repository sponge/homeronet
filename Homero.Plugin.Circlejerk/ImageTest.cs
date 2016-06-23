using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;
using Homero.Messages.Attachments;
using Homero.Utility;
using System.IO;

namespace Homero.Plugin.Circlejerk
{
    public class ImageTest : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() { "imagetest" };

        public ImageTest(IMessageBroker broker)
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
            ImageAttachment attachment = new ImageAttachment(Path.Combine(Paths.ResourceDirectory, "alligator.jpg"));
            client.ReplyTo(e.Command, "honk", attachment);
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