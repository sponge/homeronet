using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Messages;
using Homero.Core.Messages.Attachments;

namespace Homero.Core.Client.IRC
{
    public class IrcMessage : IStandardMessage
    {
        public string Message { get; set; }
        public List<IAttachment> Attachments => null; // :*(

        public IrcMessage(string message)
        {
            Message = message;
        }
    }
}
