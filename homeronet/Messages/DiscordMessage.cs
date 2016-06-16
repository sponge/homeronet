using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace homeronet.Messages
{
    public class DiscordMessage : IStandardMessage
    {
        private Message _inner;

        public DiscordMessage(Message msg)
        {
            _inner = msg;
        }


        public string Message
        {
            get { return _inner.Text; }
        }

        public string Sender
        {
            get { return _inner.User.Nickname; }
        }

        public string Target
        {
            get { return _inner.MentionedUsers?.FirstOrDefault()?.ToString(); }
        }

        public string Channel
        {
            get { return _inner.Channel.Name; }
        }

        public string Server
        {
            get { return _inner.Server.Name; }
        }

        public bool IsPrivate
        {
            get { return _inner.Channel.IsPrivate; }
        }

        public Message InnerMessage
        {
            get
            {
                return _inner;
            }
        }
    }
}
