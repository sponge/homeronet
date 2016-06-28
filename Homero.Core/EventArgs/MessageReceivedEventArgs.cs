using Homero.Core.Interface;
using Homero.Core.Messages;

namespace Homero.Core.EventArgs
{
    public class MessageReceivedEventArgs : System.EventArgs
    {
        public MessageReceivedEventArgs(IStandardMessage message)
        {
            Message = message;
        }

        public IStandardMessage Message { get; }
        public IServer Server { get; }
        public IChannel Channel { get; }
        public IUser User { get; }

        public ISendable Target
        {
            get
            {
                if (Channel != null)
                {
                    return Channel;
                }
                return User;
            }
        }
    }
}