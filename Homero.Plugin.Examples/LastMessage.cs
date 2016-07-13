using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using Homero.Plugin.Logging;
using Homero.Plugin.Logging.Entity;

namespace Homero.Plugin.Examples
{
    public class LastMessage : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() { "lastmessage" };
        public List<string> RegisteredTextCommands => _registeredCommands;

        public LastMessage(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            using (var ctx = LogContextFactory.Get((IClient) sender, e.Server))
            {
                if (ctx != null)
                {
                    Message message = ctx.Messages.OrderByDescending(x=> x.Timestamp).First(x => x.Channel.Equals(e.Channel.Name));
                    if (message != null)
                    {
                        e.ReplyTarget.Send($"{message.User} said: {message.Content}");
                    }
                    else
                    {
                        e.ReplyTarget.Send("No one said anything before you! Weird huh?");
                    }
                }
                else
                {
                    e.ReplyTarget.Send("I don't log here, sorry.");
                }
            }

        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }
    }
}
