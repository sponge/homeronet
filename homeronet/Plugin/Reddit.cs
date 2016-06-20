using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Homero.Messages;
using Newtonsoft.Json.Linq;

namespace Homero.Plugin
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

        public void Startup()
        {
            _webClient = new WebClient();
        }

        public List<string> RegisteredTextCommands
        {
            get { return _commandForName.Keys.ToList(); }
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (_commandForName.ContainsKey(command.Command))
                {
                    RedditCommand redditCommand = _commandForName[command.Command];
                    Random random = new Random();

                    string subreddit = redditCommand.Subreddits[random.Next(redditCommand.Subreddits.Count)];

                    JObject jsonParsed = JObject.Parse(_webClient.DownloadString("http://reddit.com/r/" + subreddit + ".json"));
                    List<dynamic> postList = jsonParsed["data"]["children"].ToObject<List<dynamic>>();

                    string message = postList[random.Next(postList.Count)]["data"]["title"];

                    return command.InnerMessage.CreateResponse(redditCommand.FormatMessage(message));
                }
                return null;
            });
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }

        public void Shutdown()
        {
        }
    }
}
