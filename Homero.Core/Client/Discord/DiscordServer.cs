using Discord;
using System.Collections.Generic;
using System.Linq;

namespace Homero.Core.Client.Discord
{
    public class DiscordServer : IServer
    {
        private Server _server;

        public DiscordServer(Server server)
        {
            _server = server;
        }

        public string Name
        {
            get { return _server?.Name; }
        }

        public List<IChannel> Channels
        {
            get
            {
                return _server?.AllChannels.Select(x => new DiscordChannel(x)).Cast<IChannel>().ToList();
            }
        }
    }
}