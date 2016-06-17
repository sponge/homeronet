using homeronet.Client;
using homeronet.Messages;
using homeronet.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace homeronet.Plugin {

    public class YouTube : IPlugin {

        private struct YouTubeVideo {
            public string title;
            public string length;
            public string likeCount;
            public string dislikeCount;
            public string viewCount;
            public string channelTitle;
            public string publishedAt;
            public string videoUrl;
        }

        private UriWebClient _webClient;
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

        private IClientConfiguration _youtubeConfig;

        public void Startup() {
            _webClient = new UriWebClient();
            _youtubeConfig = Program.Kernel.Get<IClientConfiguration>(new Parameter("ClientName", "YouTube", true));
        }

        private YouTubeVideo SearchVideo(string searchTerm, bool randomResult) {
            searchTerm = Uri.EscapeUriString(searchTerm);
            var json = _webClient.DownloadString(string.Format(_searchUrl, searchTerm, _youtubeConfig.ApiKey));
            var results = JsonConvert.DeserializeObject<JObject>(json);

            var items = (JArray)results.SelectToken("items");
            var rnd = _random.Next(items.Count);
            string videoId = results.SelectToken("items[" + rnd + "].id.videoId").ToString();

            var videoJson = _webClient.DownloadString(string.Format(_infoUrl, videoId, _youtubeConfig.ApiKey));
            var videoInfo = JsonConvert.DeserializeObject<JObject>(videoJson);

            var ret = new YouTubeVideo();
            ret.title = videoInfo.SelectToken("items[0].snippet.title").ToString();
            ret.length = videoInfo.SelectToken("items[0].contentDetails.duration")?.ToString().Replace("PT", "").ToLower();
            ret.likeCount = videoInfo.SelectToken("items[0].statistics.likeCount")?.ToString();
            ret.dislikeCount = videoInfo.SelectToken("items[0].statistics.dislikeCount")?.ToString();
            ret.viewCount = videoInfo.SelectToken("items[0].statistics.viewCount")?.ToString();
            ret.channelTitle = videoInfo.SelectToken("items[0].snippet.channelTitle")?.ToString();
            ret.publishedAt = videoInfo.SelectToken("items[0].snippet.publishedAt")?.ToString();
            ret.videoUrl = "http://youtu.be/" + videoId;

            return ret;
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                if (command.Command == "youtube" || command.Command == "yt") {
                    if (command.Arguments == null || command.Arguments.Count == 0) {
                        return command.InnerMessage.CreateResponse("youtube <query> -- returns the first YouTube search result for <query>");
                    }
                    else {
                        var vid = SearchVideo(string.Join(" ", command.Arguments), false);
                        // TODO: flesh this out, return a message for irc, and a message for discord that has less and markdown formatting
                        return command.InnerMessage.CreateResponse(vid.title + " - " + vid.videoUrl);
                    }
                }
                else if (command.Command == "kula") {
                    var vid = SearchVideo("kula world", true);
                    return command.InnerMessage.CreateResponse(vid.title + " - " + vid.videoUrl);
                }
                else if (command.Command == "sylauxe") {
                    var vid = SearchVideo(_sylauxeSearches[_random.Next(_sylauxeSearches.Count)], true);
                    return command.InnerMessage.CreateResponse(vid.title + " - " + vid.videoUrl);
                }

                return null;
            });
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}