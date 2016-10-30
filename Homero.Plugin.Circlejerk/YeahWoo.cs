using Homero.Core;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System.Collections.Generic;

namespace Homero.Plugin.Circlejerk
{
    public class YeahWoo : IPlugin
    {
        public YeahWoo(IMessageBroker broker)
        {
            broker.MessageReceived += BrokerOnMessageReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands => null;

        private void BrokerOnMessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.Message == "yeah")
            {
                e.ReplyTarget.Send("woo");
            }
            else if (e.Message.Message == "woo")
            {
                e.ReplyTarget.Send("yeah");
            }
        }
    }
}