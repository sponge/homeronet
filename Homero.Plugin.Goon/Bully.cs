using Homero.Core;
using Homero.Core.Client.IRC;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System.Collections.Generic;
using System.Linq;

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

        public List<string> RegisteredTextCommands { get; } = new List<string> { "bully", "bullyall" };

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            if (e.Command.Command == "bullyall")
            {
                if (e.Channel?.Users != null)
                {
                    foreach (IUser user in e.Channel?.Users)
                    {
                        if (sender is IrcClient)
                        {
                            e.ReplyTarget.Send($"{user.Mention}, I feel offended by your recent action(s). Please read http://stop-irc-bullying.eu/stop");
                        }
                        else
                        {
                            e.ReplyTarget.Send($"{user.Mention}, I feel offended by your recent action(s). Even though this isn't IRC, please read http://stop-irc-bullying.eu/stop");
                        }
                    }
                }
                return;
            }

            IUser targetUser = null;

            if (e.Command.Arguments.Count > 0)
            {
                targetUser = e.Channel?.Users?.FirstOrDefault(x => x.Name == e.Command.Arguments[0]);
            }

            if (sender is IrcClient)
            {
                e.ReplyTarget.Send($"{targetUser?.Mention}, feel offended by your recent action(s). Please read http://stop-irc-bullying.eu/stop");
            }
            else
            {
                e.ReplyTarget.Send($"{targetUser?.Mention}, I feel offended by your recent action(s). Even though this isn't IRC, please read http://stop-irc-bullying.eu/stop");
            }
        }
    }
}