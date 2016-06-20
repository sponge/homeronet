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

        private int threadNumber = 0;

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
            if (e.Command.Command == "threadtest")
            {
                int localThreadNumber = threadNumber;
                ((IClient)sender).SendMessage(e.Command.InnerMessage.CreateResponse($"Hello! I'm thread #{localThreadNumber}. I will sleep for 5 sec."));
                threadNumber += 1;
                Thread.Sleep(TimeSpan.FromSeconds(5));
                ((IClient)sender).SendMessage(e.Command.InnerMessage.CreateResponse($"Hello again! Thread #{localThreadNumber} is awake!"));

            }
            if (_tumblrMap.ContainsKey(e.Command.Command))
            {
                _webClient.DownloadString("http://" + _tumblrMap[e.Command.Command] + ".tumblr.com/random");
                ((IClient)sender).SendMessage(e.Command.InnerMessage.CreateResponse(_webClient.ResponseUri?.ToString()));
            }
        }

        private void BrokerOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _logger.Info(e.Message.Message);
        }
    }
}