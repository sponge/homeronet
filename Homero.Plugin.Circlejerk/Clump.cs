using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;
using Homero.Core.Utility;

namespace Homero.Plugin.Circlejerk
{
    public class Clump : IPlugin
    {
        private string _clump = "゜・。。・゜゜・。。・゜☆゜・。。・゜ im too bullshit feeligns  。・゜゜・。。・゜☆゜・。。・゜゜・。。・゜";

        public Clump(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> {"clump"};

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            var prefix = client?.IrcFormattingSupported == true ? "" + ControlCode.Color + ColorCode.Violet : "";
            var outStr = $"{prefix}{_clump}";
            client?.ReplyTo(e.Command, outStr);
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}