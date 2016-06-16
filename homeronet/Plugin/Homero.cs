using System;
using homeronet.Messages;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using homeronet.Utility;

namespace homeronet.Plugin
{
    public class Homero : IPlugin
    {
        private UriWebClient _webClient;
        private List<string> _registeredCommands = new List<string>() { "homero" };
        public void Startup()

        {
            Program.Log.Info("I startup, ola.");
            _webClient = new UriWebClient();
        }

        public void Shutdown()
        {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (command.Command == "homero")
                {
                    _webClient.DownloadString("http://simpsons-latino.tumblr.com/random");
                    return new StandardMessage()
                    {
                        Target = command.InnerMessage.Sender,
                        IsPrivate = command.InnerMessage.IsPrivate,
                        Channel = command.InnerMessage.Channel,
                        Message = _webClient.ResponseUri.ToString(),
                        Server = command.InnerMessage.Server
                    };
                }
                return null;
            });
        }

        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (message.Message == "hello homero")
                {
                    return new StandardMessage()
                    {
                        Target = message.Sender,
                        IsPrivate = message.IsPrivate,
                        Channel = message.Channel,
                        Message = "hi friend",
                        Server = message.Server
                    };
                }
                return null;
            });
        }
    }
}