using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using Ninject;

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
