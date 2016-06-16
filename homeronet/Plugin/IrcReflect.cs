using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Client;
using homeronet.Messages;
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

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command)
        {
            return null;
        }

        public List<string> RegisteredTextCommands { get; }
        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (message is DiscordMessage && message.Channel.Contains("sa-minecraft"))
                {

                    IClient client = Program.Kernel.Get<IrcClient>() as IClient;
                    if (client != null)
                    {
                        client.SendMessage(new StandardMessage()
                        {
                            Message = $"<{message.Sender}> {message.Message}",
                            Channel = "#sa-minecraft"
                        });
                    }
                }

                return null;
            });
        }
    }
}
