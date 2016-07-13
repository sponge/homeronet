using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;
using Homero.Core.Utility;
using Ninject;

namespace Homero.Plugin.Discord
{
    public class NowFailing : IPlugin
    {
        public List<string> RegisteredTextCommands => null;
        private DiscordClient _client;
        public NowFailing(IMessageBroker broker, IKernel kernel)
        {
            _client = kernel.Get<DiscordClient>();
            broker.CommandFailed += BrokerOnCommandFailed;
        }

        private void BrokerOnCommandFailed(object sender, EventFailedEventArgs e)
        {
            if (e.OriginalEventArgs is CommandReceivedEventArgs)
            {

                ((CommandReceivedEventArgs)e.OriginalEventArgs).ReplyTarget.Send($"AN EXCEPTION HAS BEEN THROWN FROM {e.Exception.Source}. STACKTRACE IS AS FOLLOWS. ```{e.Exception.StackTrace}```", new ImageAttachment()
                {
                    Name = "good job dipshit.png",
                    DataStream = File.OpenRead(Path.Combine(Paths.ResourceDirectory,"broke_bot.png"))
                });
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
