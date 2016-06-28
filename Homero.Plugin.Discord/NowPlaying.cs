using System.Collections.Generic;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;

namespace Homero.Plugin.Discord
{
    public class NowPlaying : IPlugin
    {
        public NowPlaying(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }


        public List<string> RegisteredTextCommands { get; } = new List<string> {"nowplaying"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            if (sender is DiscordClient)
            {
                // Dynamic this so we don't have to do assembly references.
                // Yeah, it's kind of gross but I don't want to pull in the package in to this as well.
                // Not yet anyway. I totally have intentions here to be discussed later.
                dynamic client = sender as DiscordClient;
                client?.RootClient.SetGame(string.Join(" ", e.Command.Arguments));
            }
        }
    }
}