using System;
using System.Collections.Generic;
using System.Linq;
using Homero.Core.Interface;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;

namespace Homero.Core.Client.IRC
{
    public class IrcChannel : IChannel
    {
        private IrcDotNet.IrcChannel _inner;
        private IUploader _uploader;
        public IrcChannel(IrcDotNet.IrcChannel inner, IUploader uploader)
        {
            _inner = inner;
            _uploader = uploader;
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

            List<string> messagesToSend = new List<string>();

            // split up multi-line messages
            messagesToSend.AddRange(Message.Split('\n'));

            // upload each attachment.
            if (Attachments != null)
            {
                foreach (IAttachment attachment in Attachments)
                {
                    if (attachment is ImageAttachment)
                    {
                        string image = _uploader.Upload(attachment as ImageAttachment).Result;
                        if (!string.IsNullOrEmpty(image))
                        {
                            messagesToSend.Add(image);
                        }
                    }
                }
            }

            foreach (string message in messagesToSend)
            {
                _inner.Client.LocalUser.SendMessage(_inner, message);
            }
        }

        public string Name
        {
            get
            {
                return _inner.Name;
            }
        }

        public string Topic
        {
            get
            {
                return _inner.Topic;
            }
        }

        public List<IUser> Users
        {
            get { return _inner.Users.GetUsers().Select(x => new IrcUser(x, _uploader)).Cast<IUser>().ToList(); }
        }
    }
}
