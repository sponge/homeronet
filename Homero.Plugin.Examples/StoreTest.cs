using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Homero.Plugin.Examples
{
    public class StoreTest : IPlugin
    {
        private List<string> _commands = new List<string>()
        {
            "set",
            "get",
        };

        private IStore _store;

        public StoreTest(IMessageBroker broker, IStore store)
        {
            _store = store;
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            if (e.Command.Command == "get")
            {
                string result = _store.Get<string>(string.Join(" ", e.Command.Arguments));
                e.ReplyTarget.Send(result);
            }
            if (e.Command.Command == "set")
            {
                if (e.Command.Arguments.Count >= 2)
                {
                    _store.Set<string>(e.Command.Arguments[0], string.Join(" ", e.Command.Arguments.Skip(1)));
                    e.ReplyTarget.Send("i set the thing");
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