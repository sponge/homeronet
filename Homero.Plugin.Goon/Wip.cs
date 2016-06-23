using System;
using System.Collections.Generic;
using Homero.Client;
using Homero.Services;
using HtmlAgilityPack;
using System.Linq;
using Homero.Utility;
using Homero.Messages.Attachments;

namespace Homero.Plugin.Goon {
    public class Wip : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "wip" };
        private string baseUrl = "http://www.textfiles.com/underconstruction/";

        private Random _random;
        private UriWebClient _webClient;
        private List<string> _images;

        public Wip(IMessageBroker broker) {
            _random = new Random();
            _webClient = new UriWebClient();

            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup() {
            _images = new List<string>();
        }

        public void Shutdown() {
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            // do lazy initialization of images so we dont slowdown startup for a stupid plugin
            if (_images.Count == 0) {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load("http://www.textfiles.com/underconstruction/");

                var imageNodes = doc.DocumentNode.SelectNodes("//img/@src");

                _images = (from image in imageNodes select image.GetAttributeValue("src", "help")).ToList();
            }

            var imgName = _images[_random.Next(_images.Count)];
            var imgUrl = $"{baseUrl}{imgName}";

            // TODO: fix me when pp fixes image attachments
            //if (client?.InlineOrOembedSupported == true) {
            //    var img = new ImageAttachment(imgName) { DataStream = _webClient.OpenRead(imgUrl) };
            //    client?.ReplyTo(e.Command, img);
            //} else {
                client?.ReplyTo(e.Command, imgUrl);
            //}
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }
    }
}