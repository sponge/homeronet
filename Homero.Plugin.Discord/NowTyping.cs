using Homero.Core;
using Homero.Core.Client.Discord;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using Ninject;
using System.Collections.Generic;

namespace Homero.Plugin.Discord
{
    public class NowTyping : IPlugin
    {
        public List<string> RegisteredTextCommands => null;
        private DiscordClient _client;

        public NowTyping(IMessageBroker broker, IKernel kernel)
        {
            _client = kernel.Get<DiscordClient>();
            broker.CommandDispatching += BrokerOnCommandDispatching;
        }

        private void BrokerOnCommandDispatching(object sender, CommandReceivedEventArgs e)
        {
            if (e.ReplyTarget is DiscordChannel)
            {
                ((DiscordChannel)e.ReplyTarget).SendIsTyping();
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