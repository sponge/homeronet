using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Homero.Core.Interface;

namespace Homero.Core.Client
{
    public class DiscordServer : IServer
    {
        private Server _server;
        public DiscordServer(Server server)
        {
            server = _server;
        }

        public string Name
        {
            get { return _server.Name; }
        }

        public List<IChannel> Channels
        {
            get
            {
                return _server.AllChannels.Select(x => new DiscordChannel(x)).Cast<IChannel>().ToList();
            }
        }
    }
}
