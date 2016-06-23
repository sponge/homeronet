using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Pipe
{
    public class PipePlugin : IPlugin
    {
        private List<string> commands = new List<string>() {"pip", "pipe"};
        private IMessageBroker _broker;
        public PipePlugin(IMessageBroker broker)
        {
            _broker = broker;
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;

            PipeClient pipeClient = new PipeClient(client, _broker, e.Command);
            List<string> commandChain = string.Join(" ", e.Command.Arguments).Split('|').ToList();

            for (int i = 0; i < commandChain.Count; i++)
            {
                commandChain[i] = commandChain[i].Trim();
            }
            TextCommand firstCommand = new TextCommand()
            {
                Arguments = commandChain[0].Split(' ').Skip(1).ToList(),
                Command = commandChain[0].Split(' ')[0]
            };

            commandChain.RemoveAt(0);
            pipeClient._commandChain = commandChain;
            pipeClient.FireFirstMessage(firstCommand);
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands
        {
            get
            {
                return commands;
            }
        }
    }
}
