using Homero.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Services;

namespace Homero.Plugin.Discord
{
    public class NowPlaying : IPlugin
    {
        private List<string> _registeredTextCommands = new List<string>() { "nowplaying" };

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
        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e)
        {
            if (sender is DiscordClient)
            {
                // Dynamic this so we don't have to do assembly references.
                // Yeah, it's kind of gross but I don't want to pull in the package in to this as well.
                // Not yet anyway. I totally have intentions here to be discussed later.
                dynamic client = sender as DiscordClient;
                client?.RootClient.SetGame(String.Join(" ", e.Command.Arguments));
            }
        }


        public List<string> RegisteredTextCommands
        {
            get { return _registeredTextCommands; }
        }
    }
}
