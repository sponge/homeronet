using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;

namespace Homero.Plugin.Goon
{
    public class Bully : IPlugin
    {
        public Bully(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> {"bully"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            var outStr = "";

            if (e.Command.Arguments.Count > 0)
            {
                outStr += string.Join(" ", e.Command.Arguments) + ": ";
            }

            if (sender is IrcClient)
            {
                outStr += "I feel offended by your recent action(s). Please read http://stop-irc-bullying.eu/stop";
            }
            else
            {
                outStr +=
                    "I feel offended by your recent action(s). Even though this isn't IRC, please read http://stop-irc-bullying.eu/stop";
            }

            client?.ReplyTo(e.Command, outStr);
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}