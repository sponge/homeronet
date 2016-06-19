using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Messages;

namespace homeronet.EventArgs
{
    public class MessageSentEventArgs
    {
        public IStandardMessage Message { get; set; }
        public MessageSentEventArgs(IStandardMessage message)
        {
            Message = message;
        }
    }
}
