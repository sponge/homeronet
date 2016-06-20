using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using homeronet.Client;
using homeronet.Messages.Attachments;

namespace homeronet.Messages
{
    public class DiscordMessage : IStandardMessage
    {
        private Message _inner;
        private IClient _sender;

        public DiscordMessage(IClient sender, Message msg)
        {
            _inner = msg;
            _sender = sender;
        }


        public string Message
        {
            get { return _inner.Text; }
        }

        public string Sender
        {
            get { return _inner.User?.Name; }
        }

        public string Target
        {
            get { return _inner.MentionedUsers?.FirstOrDefault()?.ToString(); }
        }

        public string Channel
        {
            get { return _inner.Channel?.Name; }
        }

        public string Server
        {
            get { return _inner.Server?.Name; }
        }

        public bool IsPrivate
        {
            get { return _inner.Channel == null || _inner.Channel.Name.StartsWith("@"); }
        }

        public List<IAttachment> Attachments
        {
            get
            {
                // Bloody expensive call. We need to back this property at some point.
                List<IAttachment> result = new List<IAttachment>();

                foreach (Message.Attachment attach in _inner.Attachments)
                {
                    result.Add(new FileAttachment()
                    {
                        Name = attach.Filename,
                        Uri = new Uri(attach.Url)
                    });
                }
                return result;
            }
        }

        public Message InnerMessage
        {
            get
            {
                return _inner;
            }
        }

        public IClient SendingClient
        {
            get
            {
                return _sender;
            }
        }

        public IStandardMessage CreateResponse(string message = null)
        {
            return new StandardMessage()
            {

                Target = this.Sender,
                IsPrivate = this.IsPrivate,
                Channel = this.Channel,
                Message = String.IsNullOrEmpty(message) ? String.Empty : message,
                Server = this.Server,
                Sender = this.Target, // not exactly true but it'll do...
            };
        }

    }
}
