using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homero.Plugin.Circlejerk
{
    public class Depths : IPlugin
    {
        public Depths(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "depths" };

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var str = string.Join(" ", e.Command.Arguments).Replace('o', 'ø');
            e.ReplyTarget.Send(str);
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}