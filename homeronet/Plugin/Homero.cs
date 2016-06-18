using homeronet.Messages;
using homeronet.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using homeronet.Services;

namespace homeronet.Plugin {

    public class Homero : IPlugin {
        private UriWebClient _webClient;
        private List<string> _registeredCommands = new List<string>() { "homero", "dog", "realbusinessmen" };
        private ILogger _logger;
        private Dictionary<String, String> _tumblrMap = new Dictionary<String, String>() {
            {"homero", "simpsons-latino"},
            {"dog", "goodassdog"},
            {"realbusinessmen", "realbusinessmen"}
        };

        public Homero(ILogger logger)
        {
            _logger = logger;
        }

        public void Startup() {
            _logger.Info("I startup, ola.");
            _webClient = new UriWebClient();
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                if (_tumblrMap.ContainsKey(command.Command)) {
                    _webClient.DownloadString("http://" + _tumblrMap[command.Command] + ".tumblr.com/random");
                    return command.InnerMessage.CreateResponse(_webClient.ResponseUri?.ToString());
                }
                return null;
            });
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return new Task<IStandardMessage>(() => {
                if (message.Message == "hello homero") {
                    return message.CreateResponse("hi friend");
                }
                return null;
            });
        }
    }
}