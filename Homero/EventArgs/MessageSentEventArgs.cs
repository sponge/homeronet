using Homero.Messages;

namespace Homero.EventArgs
{
    public class MessageSentEventArgs : System.EventArgs
    {
        public IStandardMessage Message { get; set; }
        public MessageSentEventArgs(IStandardMessage message)
        {
            Message = message;
        }
    }
}
