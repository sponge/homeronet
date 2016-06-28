using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Homero.Core.Interface;
using Homero.Core.Messages.Attachments;

namespace Homero.Core.Client
{
    public class DiscordUser : IUser
    {
        private User _user;
        public DiscordUser(User user)
        {
            _user = user;
        }
        public void Send(string Message)
        {
            Send(Message, null);
        }

        public void Send(string Message, params object[] Format)
        {
            Send(String.Format(Message,Format), null);
        }

        public void Send(string Message, params IAttachment[] Attachments)
        {
            Task<Channel> channelTask = _user.CreatePMChannel();
            channelTask.ContinueWith(delegate(Task<Channel> task)
            {
                Channel pmChannel = task.Result;
                if (Attachments != null && Attachments.Length > 0)
                {
                    foreach (IAttachment attachment in Attachments)
                    {
                        pmChannel.SendFile(attachment.Name, attachment.DataStream);
                    }
                }
                if (!String.IsNullOrEmpty(Message))
                {
                    pmChannel.SendMessage(Message);
                }
            });

        }

        public string Name
        {
            get { return _user.Name; }
        }

        public string Nickname
        {
            get { return _user.Nickname; }
        }
    }
}
