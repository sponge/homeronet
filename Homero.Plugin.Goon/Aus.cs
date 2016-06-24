using System;
using System.Collections.Generic;
using Homero.Client;
using Homero.Services;
using HtmlAgilityPack;
using System.Linq;
using Homero.Utility;
using Homero.Messages.Attachments;

namespace Homero.Plugin.Goon {
    public class Aus : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "aus" };
        private string _baseUrl = "http://www.boganipsum.com/";

        private HtmlWeb _web;

        public Aus(IMessageBroker broker) {

            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup() {
            _web = new HtmlWeb();
        }

        public void Shutdown() {
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            HtmlDocument doc = _web.Load(_baseUrl);

            var paragraph = doc.DocumentNode.SelectSingleNode("//div[@class=\"bogan-ipsum\"]/p");

            if (paragraph == null) {
                client?.ReplyTo(e.Command, "can't figure it out m8");
                return;
            }

            client?.ReplyTo(e.Command, paragraph.InnerText);
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }
    }
}