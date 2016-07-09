using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System;
using System.Collections.Generic;

namespace Homero.Plugin.Converter
{
    public class Beats : IPlugin
    {
        public Beats(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "beats" };

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            var now = DateTime.UtcNow;
            var beatsTime = Math.Floor((now.Second + (now.Minute * 60) + (now.Hour * 3600)) / 86.4f);
            e.ReplyTarget.Send($"utc time: {now.ToString("H:mm:ss")} | beat time: @{beatsTime}");
        }
    }
}