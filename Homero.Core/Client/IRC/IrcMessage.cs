using Homero.Core.Messages;
using Homero.Core.Messages.Attachments;
using System.Collections.Generic;

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