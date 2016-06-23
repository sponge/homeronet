using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Circlejerk {
    public class Clump : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "clump" };

        private string _clump = "゜・。。・゜゜・。。・゜☆゜・。。・゜ im too bullshit feeligns  。・゜゜・。。・゜☆゜・。。・゜゜・。。・゜";

        public Clump(IMessageBroker broker) {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            string prefix = client?.IrcFormattingSupported == true ? "\x0306" : "";
            var outStr = $"{prefix}{_clump}";
            client?.ReplyTo(e.Command, outStr);
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}
