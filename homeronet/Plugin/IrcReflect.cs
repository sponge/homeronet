using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Client;
using homeronet.Messages;
using homeronet.Plugins;
using Ninject;

namespace homeronet.Plugin
{
    public class IrcReflect : IPlugin
    {
        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public Task<IStandardMessage> HandleTextCommandInvocationAsync(ITextCommand command)
        {
            return null;
        }

        public List<string> RegisteredTextCommands { get; }
        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (message is DiscordMessage)
                {
                    if (message.Channel.Contains("sa-minecraft"))
                    {
                        IrcClient client = Program.Kernel.Get<IrcClient>();
                        if (client != null)
                        {
                            // Do things
                        }
                    }
                }
                return null;
            });
        }
    }
}
