using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Core.Utility
{
    public static class Web
    {
        public static string GetString(string url)
        {
            var stringTask = GetStringAsync(url).ContinueWith((task) =>
            {
                if (task.IsFaulted)
                {
                    return null;
                }
                return task.Result;
            });

            stringTask.RunSynchronously();
            return stringTask.Result;
        }

        public static Task<string> GetStringAsync(string url)
        {
            UriWebClient client = new UriWebClient();
            return client.DownloadStringTaskAsync(url);
        }

        public static JObject GetJson(string url)
        {
            var jsonTask = GetJsonAsync(url).ContinueWith((task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    return null;
                }
                return task.Result;
            });

            jsonTask.RunSynchronously();
            return jsonTask.Result;
        }

        public static Task<JObject> GetJsonAsync(string url)
        {
            Task<JObject> jsonTask = Task.Run(async () =>
            {
                string result = await GetStringAsync(url);
                if(string.IsNullOrEmpty(result))
                {
                    throw new Exception("empty json lol");
                }
                return JObject.Parse(result);
            }).ContinueWith((task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    return null;
                }
                return task.Result;
            });

            jsonTask.ConfigureAwait(true);
            return jsonTask;
        }
    }
}
