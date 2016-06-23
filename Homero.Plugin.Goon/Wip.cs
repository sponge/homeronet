using System;
using System.Collections.Generic;
using Homero.Client;
using Homero.Services;
using HtmlAgilityPack;
using System.Linq;

namespace Homero.Plugin.Goon {
    public class Wip : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "wip" };
        private string baseUrl = "http://www.textfiles.com/underconstruction/";

        private Random _random;
        private List<string> _images = new List<string>();

        public Wip(IMessageBroker broker) {
            _random = new Random();

            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup() {
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

            var outStr = $"{baseUrl}{_images[_random.Next(_images.Count)]}";
            client?.ReplyTo(e.Command, outStr);
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }
    }
}