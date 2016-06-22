using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Services;
using Newtonsoft.Json.Linq;

namespace Homero.Plugin.Circlejerk
{
    public class Reddit : IPlugin
    {
        private class RedditCommand
        {
            public List<string> Subreddits { get; set; }
            public Func<string, string> FormatMessage { get; set; }
        }

        private WebClient _webClient;
        private Dictionary<string, RedditCommand> _commandForName = new Dictionary<string, RedditCommand>()
        {
            {
                "alligator", new RedditCommand()
                {
                    Subreddits =  new List<string>() { "britishproblems" },
                    FormatMessage =  message => "<alligator> " + message.ToLower()
                }
            },
            {
                "danl", new RedditCommand()
                {
                    Subreddits =  new List<string>() { "TheRedPill", "seduction", "CuckoldCommunity", "BikePorn", "electronic_cigarette", "snooker" },
                    FormatMessage = message => "<bill`gate> " + message.ToLower()
                }
            },
            {
                "hurt", new RedditCommand()
                {
                    Subreddits = new List<string>() { "Buddhism", "explainlikeimfive", "gifs", "Eve", "weightlifting", "MensRights", "hardbodies", "DoesAnybodyElse" },
                    FormatMessage = message => "<Hurt> " + message
                }
            }
        };

        public Reddit(IMessageBroker broker)
        {
            _webClient = new WebClient();
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public List<string> RegisteredTextCommands
        {
            get { return _commandForName.Keys.ToList(); }
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            RedditCommand redditCommand = _commandForName[e.Command.Command];
            Random random = new Random();

            string subreddit = redditCommand.Subreddits[random.Next(redditCommand.Subreddits.Count)];

            JObject jsonParsed = JObject.Parse(_webClient.DownloadString("http://reddit.com/r/" + subreddit + ".json"));
            List<dynamic> postList = jsonParsed["data"]["children"].ToObject<List<dynamic>>();

            string message = postList[random.Next(postList.Count)]["data"]["title"];

            client?.ReplyTo(e.Command, redditCommand.FormatMessage(message));
        }

        public void Shutdown()
        {
        }
    }
}
