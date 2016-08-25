using Homero.Core.EventArgs;
using Homero.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Homero.Plugin.Goon
{
    public class Dominions : IPlugin
    {
        private const string BASE_URL =
            "http://larzm42.github.io/dom4inspector/?page={0}&{0}q={1}&showmoddinginfo=1&showids=1";

        private Dictionary<string, string> _pageNameMappings = new Dictionary<string, string>
        {
            {"item", "item"},
            {"hat", "item"},
            {"spell", "spell"},
            {"unit", "unit"},
            {"site", "site"},
            {"weapon", "wpn"},
            {"armor", "armor"},
            {"merc", "merc"},
            {"event", "event"}
        };

        public Dominions(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public List<string> RegisteredTextCommands
        {
            get { return new List<string> { "dom4" }; }
        }

        public void Shutdown()
        {
        }

        public void Startup()
        {
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            if (e.Command.Arguments?.Count >= 2)
            {
                var thing = e.Command.Arguments.ElementAt(0);
                var query = string.Join(" ", e.Command.Arguments.Skip(1));
                if (_pageNameMappings.ContainsKey(thing))
                {
                    e.ReplyTarget.Send(string.Format(BASE_URL, _pageNameMappings[thing], HttpUtility.UrlEncode(query)));
                }
            }
        }
    }
}