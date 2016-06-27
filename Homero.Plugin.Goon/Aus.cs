using System.Collections.Generic;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using HtmlAgilityPack;

namespace Homero.Plugin.Goon
{
    public class Aus : IPlugin
    {
        private string _baseUrl = "http://www.boganipsum.com/";

        private HtmlWeb _web;

        public Aus(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
            _web = new HtmlWeb();
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> {"aus"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            var doc = _web.Load(_baseUrl);

            var paragraph = doc.DocumentNode.SelectSingleNode("//div[@class=\"bogan-ipsum\"]/p");

            if (paragraph == null)
            {
                client?.ReplyTo(e.Command, "can't figure it out m8");
                return;
            }

            client?.ReplyTo(e.Command, paragraph.InnerText);
        }
    }
}