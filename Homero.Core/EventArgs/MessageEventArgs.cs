using Homero.Core.Messages;

namespace Homero.Core.EventArgs
{
    public class MessageEventArgs : System.EventArgs
    {
        public IStandardMessage Message { get; private set; }

        public IUser User { get; private set; }

        public IChannel Channel { get; private set; }

        public IServer Server { get; private set; }

        public ISendable ReplyTarget => Channel ?? (ISendable)User;

        public MessageEventArgs(IStandardMessage message, IServer server, IChannel channel, IUser user)
        {
            Message = message;
            Server = server;
            Channel = channel;
            User = user;
        }
    }
}