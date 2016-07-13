using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;
using Homero.Core.Utility;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Homero.Plugin.Goon
{
    public class Wip : IPlugin
    {
        private List<string> _images;

        private Random _random;
        private UriWebClient _webClient;
        private string baseUrl = "http://www.textfiles.com/underconstruction/";

        public Wip(IMessageBroker broker)
        {
            _random = new Random();
            _webClient = new UriWebClient();

            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
            _images = new List<string>();
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "wip" };

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;
            // do lazy initialization of images so we dont slowdown startup for a stupid plugin
            if (_images.Count == 0)
            {
                var web = new HtmlWeb();
                var doc = web.Load("http://www.textfiles.com/underconstruction/");

                var imageNodes = doc.DocumentNode.SelectNodes("//img/@src");

                _images = (from image in imageNodes select image.GetAttributeValue("src", "help")).ToList();
            }

            var imgName = _images[_random.Next(_images.Count)];
            var imgUrl = $"{baseUrl}{imgName}";

            if (client?.InlineOrOembedSupported == true)
            {
                var img = new ImageAttachment()
                {
                    Name = "UNDERCONSTRUCTION.gif",
                    DataStream = _webClient.OpenRead(imgUrl)
                };
                e.ReplyTarget.Send(String.Empty, img);
            }
            else
            {
                e.ReplyTarget.Send(imgUrl);
            }
        }
    }
}