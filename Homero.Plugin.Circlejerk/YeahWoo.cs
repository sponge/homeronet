using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Circlejerk {

    public class YeahWoo : IPlugin
    {
        public YeahWoo(IMessageBroker broker)
        {
            broker.MessageReceived += BrokerOnMessageReceived;
        }


        public void Startup() {
        }

        public void Shutdown() {
        }


        public List<string> RegisteredTextCommands => null;

        private void BrokerOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            if (e.Message.Message == "yeah")
            {
                client?.ReplyTo(e.Message,"woo");
            }
            else if (e.Message.Message == "woo")
            {
                client?.ReplyTo(e.Message, "yeah");
            }
        }
    }
}