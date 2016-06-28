using Homero.Core.Interface;
using Homero.Core.Messages;

namespace Homero.Core.EventArgs
{
    public class MessageSentEventArgs : System.EventArgs
    {
        public MessageSentEventArgs(IStandardMessage message)
        {
            Message = message;
        }

        public IStandardMessage Message { get; }
        public IServer Server { get; }
        public IChannel Channel { get; }
        public IUser User { get; }
    }
}