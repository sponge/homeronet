using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Services;
using Homero.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Homero.Plugin.Media {

    public class YouTube : IPlugin
    {
        private WebClient _webClient;
        private Random _random = new Random();



        private List<string> _registeredCommands = new List<string>() { "youtube", "yt", "kula", "sylauxe" };

        private List<string> _sylauxeSearches = new List<string>() {
            "diaper",
            "anime",
            "my little sister cant be this cute",
            "pony",
            "anime obama"
        };

        private string _ytApiKey = String.Empty;
        private IConfiguration config;
        public YouTube(IMessageBroker broker, IConfiguration config)
        {
            this.config = config;
            _ytApiKey = config.GetValue<string>("ApiKey");
            _webClient = new WebClient();
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            YouTubeVideo video = null;

            if (e.Command.Command == "youtube" || e.Command.Command == "yt")
            {
                // TODO: update this with the proper check once lilpp decides on api design
                if (e.Command.Arguments.Count <= 0)
                {
                    client?.ReplyTo(e.Command, "youtube <query> -- returns the first YouTube search result for <query>");
                    return;
                }

                video = YouTubeVideo.Search(string.Join(" ", e.Command.Arguments), _ytApiKey);
            }
            else if (e.Command.Command == "kula")
            {
                video = YouTubeVideo.Search("kula world", _ytApiKey, true);
            }
            else if (e.Command.Command == "sylauxe")
            {
                video = YouTubeVideo.Search(_sylauxeSearches[_random.Next(_sylauxeSearches.Count)], _ytApiKey, true);
            }

            if (video != null)
            {
                if (client?.InlineOrOembedSupported == true)
                {
                    client.ReplyTo(e.Command, $"{video.Title} - {video.VideoUrl}");
                }
                else
                {
                    client?.ReplyTo(e.Command, $"{video.Title} - {video.VideoUrl} - 👍 {video.LikeCount} 	👎 {video.DislikeCount} - {video.ViewCount} views - {video.ChannelTitle} on {video.PublishedAt}");
                }
            }
        }

        public void Startup()
        {
            if (String.IsNullOrEmpty(_ytApiKey))
            {
                config.SetValue("ApiKey", "SETANAPIKEYDINGUS");
                throw new Exception("No API key provided!");
            }
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

    }
}
 