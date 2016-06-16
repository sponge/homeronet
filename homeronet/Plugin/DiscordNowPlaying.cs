using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Client;
using homeronet.Messages;

namespace homeronet.Plugin
{
    public class DiscordNowPlaying : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() { "nowplaying" };
        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (command.InnerMessage.SendingClient is DiscordClient)
                {
                    if (command.Arguments?.Count > 0)
                    {
                        DiscordClient client = command.InnerMessage.SendingClient as DiscordClient;
                        client.RootClient.SetGame(String.Join(" ", command.Arguments));
                        return command.InnerMessage.CreateResponse($"Changed playing game to: {client.RootClient.CurrentGame}");
                    }
                }
                return null;
            });
        }

        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}
