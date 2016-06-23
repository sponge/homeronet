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
            List<string> commands = string.Join(" ", e.Command.Arguments).Split('|').ToList();

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i] = commands[i].Trim();
            }
            TextCommand firstCommand = new TextCommand()
            {
                Arguments = commands[0].Split(' ').Skip(1).ToList(),
                Command = commands[0].Split(' ')[0]
            };

            commands.RemoveAt(0);
            pipeClient._commandChain = commands;
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
