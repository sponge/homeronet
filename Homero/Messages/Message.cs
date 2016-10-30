using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero
{
    public class Message : IMessage
    {
        public string Text { get; set; }
        public List<IAttachment> Attachments { get; set; }

        public Message(string text)
        {
            Text = text;
        }

        public Message(string text, List<IAttachment> attachments) : this(text)
        {
            Attachments = attachments;
        }

        public Message(IMessage message)
        {
            Text = message.Text;
            Attachments = message.Attachments;
        }
    }
}