using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System;
using System.Collections.Generic;
using System.Net;

namespace Homero.Plugin.Media
{
    public class YouTube : IPlugin
    {
        private Random _random = new Random();

        private List<string> _sylauxeSearches = new List<string>
        {
            "diaper",
            "anime",
            "my little sister cant be this cute",
            "pony",
            "anime obama"
        };

        private WebClient _webClient;

        private string _ytApiKey = string.Empty;
        private IConfiguration config;

        public YouTube(IMessageBroker broker, IConfiguration config)
        {
            this.config = config;
            _ytApiKey = config.GetValue<string>("ApiKey");
            _webClient = new WebClient();
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
            if (string.IsNullOrEmpty(_ytApiKey))
            {
                config.SetValue("ApiKey", "SETANAPIKEYDINGUS");
                throw new Exception("No API key provided!");
            }
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "youtube", "yt", "kula", "sylauxe" };

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;
            YouTubeVideo video = null;

            if (e.Command.Command == "youtube" || e.Command.Command == "yt")
            {
                if (e.Command.Arguments.Count == 0)
                {
                    e.ReplyTarget.Send("youtube <query> -- returns the first YouTube search result for <query>");
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
                    e.ReplyTarget.Send($"{video.Title} - {video.VideoUrl}");
                }
                else
                {
                    e.ReplyTarget.Send($"{video.Title} - {video.VideoUrl} - 👍 {video.LikeCount} 	👎 {video.DislikeCount} - {video.ViewCount} views - {video.ChannelTitle} on {video.PublishedAt}");
                }
            }
        }
    }
}