using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Homero.Plugin.Media
{
    public class YouTubeVideo
    {
        private static string SEARCH_URL_FORMAT =
            "https://www.googleapis.com/youtube/v3/search?part=snippet&type=video&maxResults=50&q={0}&key={1}";

        private static string INFO_URL_FORMAT =
            "https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails,statistics&id={0}&key={1}";

        public string ChannelTitle;
        public string DislikeCount;
        public string Length;
        public string LikeCount;
        public string PublishedAt;

        public string Title;
        public string VideoUrl;
        public string ViewCount;

        public static YouTubeVideo Search(string searchTerm, string apiKey, bool randomResult = false)
        {
            var client = new WebClient();
            var rng = new Random();
            searchTerm = Uri.EscapeUriString(searchTerm);
            var json = client.DownloadString(string.Format(SEARCH_URL_FORMAT, searchTerm, apiKey));
            var results = JsonConvert.DeserializeObject<JObject>(json);

            var items = (JArray) results.SelectToken("items");
            var videoId = string.Empty;

            if (randomResult)
            {
                videoId = results.SelectToken($"items[{rng.Next(items.Count)}].id.videoId").ToString();
            }
            else
            {
                videoId = results.SelectToken("items[0].id.videoId").ToString();
            }

            var videoJson = client.DownloadString(string.Format(INFO_URL_FORMAT, videoId, apiKey));
            var videoInfo = JsonConvert.DeserializeObject<JObject>(videoJson);

            return new YouTubeVideo
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
        }
    }
}