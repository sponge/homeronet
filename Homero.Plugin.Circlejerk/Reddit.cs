using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Homero.Plugin.Circlejerk
{
    public class Reddit : IPlugin
    {
        private Dictionary<string, RedditCommand> _commandForName = new Dictionary<string, RedditCommand>
        {
            {
                "alligator", new RedditCommand
                {
                    Subreddits = new List<string> {"britishproblems"},
                    FormatMessage = message => "<alligator> " + message.ToLower()
                }
            },
            {
                "danl", new RedditCommand
                {
                    Subreddits =
                        new List<string>
                        {
                            "TheRedPill",
                            "seduction",
                            "CuckoldCommunity",
                            "BikePorn",
                            "electronic_cigarette",
                            "snooker"
                        },
                    FormatMessage = message => "<bill`gate> " + message.ToLower()
                }
            },
            {
                "hurt", new RedditCommand
                {
                    Subreddits =
                        new List<string>
                        {
                            "Buddhism",
                            "explainlikeimfive",
                            "gifs",
                            "Eve",
                            "weightlifting",
                            "MensRights",
                            "hardbodies",
                            "DoesAnybodyElse"
                        },
                    FormatMessage = message => "<Hurt> " + message
                }
            }
        };

        private WebClient _webClient;

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

        public void Shutdown()
        {
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var redditCommand = _commandForName[e.Command.Command];
            var random = new Random();

            var subreddit = redditCommand.Subreddits[random.Next(redditCommand.Subreddits.Count)];

            var jsonParsed = JObject.Parse(_webClient.DownloadString("http://reddit.com/r/" + subreddit + ".json"));
            var postList = jsonParsed["data"]["children"].ToObject<List<dynamic>>();

            string message = postList[random.Next(postList.Count)]["data"]["title"];

            e.ReplyTarget.Send(redditCommand.FormatMessage(message));
        }

        private class RedditCommand
        {
            public List<string> Subreddits { get; set; }

            public Func<string, string> FormatMessage { get; set; }
        }
    }
}