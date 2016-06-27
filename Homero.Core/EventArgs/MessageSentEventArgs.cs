using Homero.Core.Messages;

namespace Homero.Core.EventArgs
{
    public class MessageSentEventArgs : System.EventArgs
    {
        public MessageSentEventArgs(IStandardMessage message)
        {
            Message = message;
        }

        public IStandardMessage Message { get; set; }
    }
}