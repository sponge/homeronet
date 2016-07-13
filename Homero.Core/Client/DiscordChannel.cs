using Discord;
using Homero.Core.Interface;
using Homero.Core.Messages.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Homero.Core.Client
{
    public class DiscordChannel : IChannel
    {
        private Channel _channel;

        public DiscordChannel(Channel channel)
        {
            _channel = channel;
        }

        public void Send(string Message)
        {
            Send(Message, null);
        }

        public void Send(string Message, params object[] Format)
        {
            Send(String.Format(Message, Format), null);
        }

        public void Send(string Message, params IAttachment[] Attachments)
        {
            if (Attachments != null && Attachments.Length > 0)
            {
                foreach (IAttachment attachment in Attachments)
                {
                    _channel.SendFile(attachment.Name, attachment.DataStream);
                }
            }

            if (!String.IsNullOrEmpty(Message))
            {
                _channel.SendMessage(Message);
            }
        }

        public string Name
        {
            get { return _channel.Name; }
        }

        public List<IUser> Users
        {
            get
            {
                return new List<IUser>(_channel.Users.Select(x => new DiscordUser(x)));
            }
        }

        public void SendIsTyping()
        {
            _channel?.SendIsTyping();
        }

    }
}