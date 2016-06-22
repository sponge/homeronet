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

        private static string _searchUrl = "https://www.googleapis.com/youtube/v3/search?part=snippet&type=video&maxResults=50&q={0}&key={1}";
        private static string _infoUrl = "https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails,statistics&id={0}&key={1}";

        private List<string> _registeredCommands = new List<string>() { "youtube", "yt", "kula", "sylauxe" };

        private List<string> _sylauxeSearches = new List<string>() {
            "diaper",
            "anime",
            "my little sister cant be this cute",
            "pony",
            "anime obama"
        };

        private string _ytApiKey = String.Empty;

        public YouTube(IMessageBroker broker, IConfiguration config)
        {
            _ytApiKey = config.GetValue<string>("ApiKey");
            if (String.IsNullOrEmpty(_ytApiKey))
            {
                config.SetValue("ApiKey", "SETANAPIKEYDINGUS");
                throw new Exception("No API key provided!");
            }
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

                video = SearchVideo(string.Join(" ", e.Command.Arguments), false);
            }
            else if (command.Command == "kula")
            {
                video = SearchVideo("kula world", true);
                return command.InnerMessage.CreateResponse($"{vid.title} - {vid.videoUrl}");
            }
            else if (command.Command == "sylauxe")
            {
                video = SearchVideo(_sylauxeSearches[_random.Next(_sylauxeSearches.Count)], true);
            }

            if (video != null)
            {
                if (client?.InlineOrOembedSupported == true)
                {
                    client.ReplyTo(e.Command, $"{video.Title} - {video.VideoUrl}");
                }
                else
                {
                    client?.ReplyTo(e.Command, $"{vid.title} - {vid.videoUrl} - 👍 {vid.likeCount} 	👎 {vid.dislikeCount} - {vid.viewCount} views - {vid.channelTitle} on {vid.publishedAt}");
                }
            }

        }

        public void Startup() {
        }

        private YouTubeVideo SearchVideo(string searchTerm, bool randomResult) {
            searchTerm = Uri.EscapeUriString(searchTerm);
            var json = _webClient.DownloadString(string.Format(_searchUrl, searchTerm, _ytApiKey));
            var results = JsonConvert.DeserializeObject<JObject>(json);

            var items = (JArray)results.SelectToken("items");
            var rnd = _random.Next(items.Count);
            string videoId = results.SelectToken($"items[{rnd}].id.videoId").ToString();

            var videoJson = _webClient.DownloadString(string.Format(_infoUrl, videoId, _ytApiKey));
            var videoInfo = JsonConvert.DeserializeObject<JObject>(videoJson);

            var ret = new YouTubeVideo
            {
                Title = videoInfo.SelectToken("items[0].snippet.title").ToString(),
                Length =
                    videoInfo.SelectToken("items[0].contentDetails.duration")?.ToString().Replace("PT", "").ToLower(),
                LikeCount = videoInfo.SelectToken("items[0].statistics.likeCount")?.ToString(),
                DislikeCount = videoInfo.SelectToken("items[0].statistics.dislikeCount")?.ToString(),
                ViewCount = videoInfo.SelectToken("items[0].statistics.viewCount")?.ToString(),
                ChannelTitle = videoInfo.SelectToken("items[0].snippet.channelTitle")?.ToString(),
                PublishedAt = videoInfo.SelectToken("items[0].snippet.publishedAt")?.ToString(),
                VideoUrl = $"http://youtu.be/{videoId}"
            };

            return ret;
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

    }
}
 