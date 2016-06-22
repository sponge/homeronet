using Homero.Messages;

namespace Homero.EventArgs
{
    public class MessageReceivedEventArgs : System.EventArgs
    {
        public IStandardMessage Message { get; set; }
        
        public MessageReceivedEventArgs(IStandardMessage message)
        {
            Message = message;
        }
        }
}
