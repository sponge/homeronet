using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Ninject;

namespace Homero.Plugin
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
                        client.DispatchMessage(new StandardMessage()
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
