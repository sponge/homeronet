using Homero.Core;
using Homero.Core.Client.Discord;
using Homero.Core.EventArgs;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;
using Homero.Core.Utility;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;

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
            if (e.OriginalEventArgs is CommandReceivedEventArgs && sender is DiscordClient)
            {
                CommandReceivedEventArgs args = e.OriginalEventArgs as CommandReceivedEventArgs;
                string message = String.Empty;
                if (args.Channel.Name.Contains("homero-dev"))
                {
                    message =
                        $"AN EXCEPTION HAS BEEN THROWN FROM {e.Exception.Source}. STACKTRACE IS AS FOLLOWS. ```{e.Exception.StackTrace}```";
                }
                args.ReplyTarget.Send(message, new ImageAttachment()
                {
                    Name = "good job dipshit.png",
                    DataStream = File.OpenRead(Path.Combine(Paths.ResourceDirectory, "broke_bot.png"))
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