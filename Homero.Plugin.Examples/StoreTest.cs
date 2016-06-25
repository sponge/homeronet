using System;
using System.Collections.Generic;
using System.Linq;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;

namespace Homero.Plugin.Examples
{
    public class StoreTest : IPlugin
    {
        private List<string> _commands = new List<string>()
        {
            "set",
            "get",
            "complexset"
        };

        private IStore _store;
        public StoreTest(IMessageBroker broker, IStore store)
        {
            _store = store;
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;

            if (e.Command.Command == "get")
            {
                string result = _store.Get<string>(string.Join(" ", e.Command.Arguments));
                client?.ReplyTo(e.Command, result);
            }
            if (e.Command.Command == "set")
            {
                if (e.Command.Arguments.Count >= 2)
                {
                    _store.Set<string>(e.Command.Arguments[0], string.Join(" ", e.Command.Arguments.Skip(1)));
                    client?.ReplyTo(e.Command, "set");
                }
            }
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
                return _commands;
            }
        }
    }
}