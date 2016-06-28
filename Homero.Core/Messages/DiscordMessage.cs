using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Homero.Core.Client;
using Homero.Core.Messages.Attachments;

namespace Homero.Core.Messages
{
    public class DiscordMessage : IStandardMessage
    {
        public DiscordMessage(IClient sender, Message msg)
        {
            InnerMessage = msg;
            SendingClient = sender;
        }

        public Message InnerMessage { get; }

        public IClient SendingClient { get; }


        public string Message
        {
            get { return InnerMessage.Text; }
        }

        public string Sender
        {
            get { return InnerMessage.User?.Name; }
        }

        public string Target
        {
            get { return InnerMessage.MentionedUsers?.FirstOrDefault()?.ToString(); }
        }

        public string Channel
        {
            get { return InnerMessage.Channel?.Name; }
        }

        public string Server
        {
            get { return InnerMessage.Server?.Name; }
        }

        public bool IsPrivate
        {
            get { return InnerMessage.Channel == null || InnerMessage.Channel.Name.StartsWith("@"); }
        }

        public List<IAttachment> Attachments
        {
            get
            {
                // Bloody expensive call. We need to back this property at some point.
                var result = new List<IAttachment>();

                foreach (var attach in InnerMessage.Attachments)
                {
                    result.Add(new FileAttachment
                    {
                        Name = attach.Filename,
                        Uri = new Uri(attach.Url)
                    });
                }
                return result;
            }
        }

        public IStandardMessage CreateResponse(string message = null)
        {
            return new StandardMessage
            {
                Target = Sender,
                IsPrivate = IsPrivate,
                Channel = Channel,
                Message = string.IsNullOrEmpty(message) ? string.Empty : message,
                Server = Server,
                Sender = Target // not exactly true but it'll do...
            };
        }
    }
}