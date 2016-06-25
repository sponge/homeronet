using Homero.Core.Messages;

namespace Homero.Core.EventArgs
{
    public class MessageReceivedEventArgs : System.EventArgs
    {
        public MessageReceivedEventArgs(IStandardMessage message)
        {
            Message = message;
        }

        public IStandardMessage Message { get; set; }
    }
}