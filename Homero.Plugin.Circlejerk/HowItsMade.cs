using System;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using Homero.Core.Utility;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Homero.Plugin
{
    public class HowItsMade : IPlugin
    {
        private ILogger _logger;
        private UriWebClient _webClient;

        public HowItsMade(ILogger logger, IMessageBroker broker)
        {
            _logger = logger;
            _webClient = new UriWebClient();

            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
            // ignored
        }

        public void Shutdown()
        {
            // ignored
        }

        public List<string> RegisteredTextCommands {
            get { return new List<string> { "made", "howitsmade" }; }
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
           
            var response = JsonConvert.DeserializeObject<JObject>(_webClient.DownloadString("http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag=how+its+made"));
            var url = response["data"]["image_url"];
            e.ReplyTarget.Send(url.ToString());
        }
    }
}