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
                    return command.InnerMessage.CreateResponse(_webClient.ResponseUri?.ToString());
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
                    return message.CreateResponse("hi friend");
                }
                return null;
            });
        }
    }
}