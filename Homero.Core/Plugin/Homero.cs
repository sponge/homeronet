using System.Collections.Generic;
using System.Linq;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using Homero.Core.Utility;

namespace Homero.Plugin
{
    public class Homero : IPlugin
    {
        private readonly Dictionary<string, string> _tumblrMap = new Dictionary<string, string>
        {
            {"homero", "simpsons-latino"},
            {"dog", "goodassdog"},
            {"realbusinessmen", "realbusinessmen"}
        };

        private ILogger _logger;
        private UriWebClient _webClient;

        public Homero(ILogger logger, IMessageBroker broker)
        {
            _logger = logger;
            _webClient = new UriWebClient();

            broker.MessageReceived += BrokerOnMessageReceived;
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

        public List<string> RegisteredTextCommands
        {
            get { return _tumblrMap.Keys.ToList(); }
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;
            _webClient.DownloadString("http://" + _tumblrMap[e.Command.Command] + ".tumblr.com/random");
            client?.ReplyTo(e.Command, _webClient.ResponseUri?.ToString());
        }

        private void BrokerOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _logger.Info(e.Message.Message);
        }
    }
}