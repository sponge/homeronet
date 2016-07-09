using Homero.Core.EventArgs;
using Homero.Core.Services;
using HtmlAgilityPack;
using System.Collections.Generic;

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

        public List<string> RegisteredTextCommands { get; } = new List<string> { "aus" };

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var doc = _web.Load(_baseUrl);

            var paragraph = doc.DocumentNode.SelectSingleNode("//div[@class=\"bogan-ipsum\"]/p");

            if (paragraph == null)
            {
                e.ReplyTarget.Send("can't figure it out m8");
                return;
            }

            e.ReplyTarget.Send(paragraph.InnerText);
        }
    }
}