using Homero.Core.EventArgs;
using Homero.Core.Services;
using System.Collections.Generic;

namespace Homero.Plugin.Pipe
{
    public class PipePlugin : IPlugin
    {
        private IMessageBroker _broker;

        public PipePlugin(IMessageBroker broker)
        {
            _broker = broker;
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "pip", "pipe" };

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            /*
            var client = sender as IClient;

            var pipeClient = new PipeClient(client, _broker, e.Command);
            var commandChain = string.Join(" ", e.Command.Arguments).Split('|').ToList();

            for (var i = 0; i < commandChain.Count; i++)
            {
                commandChain[i] = commandChain[i].Trim();
            }
            var firstCommand = new TextCommand
            {
                Arguments = commandChain[0].Split(' ').Skip(1).ToList(),
                Command = commandChain[0].Split(' ')[0]
            };

            commandChain.RemoveAt(0);
            pipeClient.CommandChain = commandChain;
            pipeClient.FireFirstMessage(firstCommand);
            */
        }
    }
}