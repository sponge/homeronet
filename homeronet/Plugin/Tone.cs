using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin
{
    public class Tone : IPlugin
    {
        // TODO: Genuine corruption
        private List<string> _tonyOutput = new List<string>()
        {
            "Tony is sitting opposite you, examinig each of his fingers in turn.",
            "You wish you could put Tony out of his misery.",
            "You isl you could put Tony out of h5s mgsery.",
            "You wis5 yougco6ld put T4ny out ofchis 4iser7.",
            "Yo4 wis5hyobgcokldsp4t T46y 3ut ofc7is 4is5r74",
            "Y44 wis5hy3bg5o5ld7p44 T464444444fc7is44i454744",
            "54783il5hy3bg5o55d788888864444444f37is24i454744",
            "----------- rest in peace tony -----------"
        };
        private List<string> _registeredCommands = new List<string>()
        {
            "tone"
        };

        public Tone(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            if (e.Command.Command == "tone")
            {
                // Don't hold up dispatch at all and run this long thing as a task.
                Task.Run(() =>
                {
                    foreach (string msg in _tonyOutput)
                    {
                        client?.ReplyTo(e.Command, msg);
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }

                });

            }
        }


        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}
