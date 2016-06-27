using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.Messages;
using Ninject;

namespace Homero.Plugin
{
    public class IrcReflect : IPlugin
    {
        private IKernel _boundKernel;

        public IrcReflect(IKernel kernel)
        {
            _boundKernel = kernel;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command)
        {
            return null;
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (message is DiscordMessage && message.Channel.Contains("sa-minecraft"))
                {
                    var client = _boundKernel.Get<IrcClient>() as IClient;
                    if (client != null)
                    {
                        client.DispatchMessage(new StandardMessage
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