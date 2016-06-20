using homeronet.Messages;
using homeronet.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using homeronet.Client;
using homeronet.EventArgs;
using homeronet.Messages.Attachments;
using homeronet.Services;

namespace homeronet.Plugin
{

    public class Homero : IPlugin
    {
        private UriWebClient _webClient;
        private List<string> _registeredCommands = new List<string>() { "homero", "dog", "realbusinessmen" };
        private ILogger _logger;
        private Dictionary<String, String> _tumblrMap = new Dictionary<String, String>() {
            {"homero", "simpsons-latino"},
            {"dog", "goodassdog"},
            {"realbusinessmen", "realbusinessmen"}
        };

        public Homero(ILogger logger, IMessageBroker broker)
        {
            _logger = logger;
            broker.MessageReceived += BrokerOnMessageReceived;
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
            _logger.Info("I startup, ola.");
            _webClient = new UriWebClient();
        }

        public void Shutdown()
        {
            // ignored
        }
        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            if (e.Command.Command == "attachtest")
            {
                client?.ReplyTo(e.Command, new ImageAttachment("D:\\toadie.jpg"));
            }
            if (_tumblrMap.ContainsKey(e.Command.Command))
            {
                _webClient.DownloadString("http://" + _tumblrMap[e.Command.Command] + ".tumblr.com/random");
                client?.ReplyTo(e.Command, _webClient.ResponseUri?.ToString());
            }
        }

        private void BrokerOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _logger.Info(e.Message.Message);
        }
    }
}