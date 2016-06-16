using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Client;
using homeronet.Messages;

namespace homeronet.EventArgs
{
    public class MessageReceivedEventArgs
    {
        public IStandardMessage Message { get; set; }

        public MessageReceivedEventArgs(IStandardMessage message)
        {
            Message = message;
        }
    }
}
