using Homero.Core.Interface;
using Homero.Core.Messages;

namespace Homero.Core.EventArgs
{
    public class CommandReceivedEventArgs : System.EventArgs
    {
        public ITextCommand Command { get; private set; }

        public IUser User { get; private set; }

        public IChannel Channel { get; private set; }

        public IServer Server { get; private set; }

        public ISendable ReplyTarget => Channel ?? (ISendable)User;

        public CommandReceivedEventArgs(ITextCommand command, IServer server, IChannel channel, IUser user)
        {
            Command = command;
            Server = server;
            Channel = channel;
            User = user;
        }
    }
}