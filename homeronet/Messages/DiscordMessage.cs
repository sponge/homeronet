using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using homeronet.Client;

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
                SendingClient = this.SendingClient
            };
        }

    }
}
