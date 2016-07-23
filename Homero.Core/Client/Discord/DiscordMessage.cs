using System;
using System.Collections.Generic;
using Discord;
using Homero.Core.Messages;
using Homero.Core.Messages.Attachments;

namespace Homero.Core.Client.Discord
{
    public class DiscordMessage : IStandardMessage
    {
        private Message _message;

        public DiscordMessage(Message message)
        {
            _message = message;
        }

        public string Message
        {
            get { return _message.Text; }
        }

        public List<IAttachment> Attachments
        {
            get
            {
                List<IAttachment> result = new List<IAttachment>();
                foreach (Message.Attachment attachment in _message.Attachments)
                {
                    FileAttachment tempAttachment = new FileAttachment() {Name = attachment.Filename, Uri = new Uri(attachment.Url)};
                    result.Add(tempAttachment);
                }
                return result;
            }
        }
    }
}
