using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;
using Homero.Core.Utility;

namespace Homero.Plugin.Circlejerk
{
    public class Hate : IPlugin
    {
        private Random _random;

        public Hate(IMessageBroker broker)
        {
            _random = new Random();

            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> {"lilpp", "sponge"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            var lines = File.ReadAllLines(Path.Combine(Paths.ResourceDirectory, $"{e.Command.Command}_list.txt"));

            var i = _random.Next(lines.Length);

            if (e.Command.Command == "lilpp")
            {
                client?.ReplyTo(e.Command, $"<LilPP> i hate {lines[i]}");
            }
            else if (e.Command.Command == "sponge")
            {
                var verb = i%3 == 0 ? "love" : i%3 == 1 ? "am ambivalent towards" : "hate";
                client?.ReplyTo(e.Command, $"<sponge> i {verb} {lines[i]}");
            }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}