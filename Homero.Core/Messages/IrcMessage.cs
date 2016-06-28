using System;
using System.Collections.Generic;
using IrcDotNet;
using Homero.Core.Messages;
using Homero.Core.Client;
using Homero.Core.Messages.Attachments;

namespace Homero.Messages
{
    public class IrcMessage : IStandardMessage
    {
        private IrcMessageEventArgs _inner;
        private IClient _sender;
        private IrcChannel _channel;

        public IrcMessage(IClient sender, IrcMessageEventArgs message, IrcChannel channel)
        {
            _sender = sender;
            _inner = message;
            _channel = channel;
        }

        public List<IAttachment> Attachments
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Channel
        {
            get { return _channel.Name; }
        }

        public bool IsPrivate => false;

        public string Message
        {
            get { return _inner.Text; }
        }

        public string Sender
        {
            get { return _inner.Source.Name; }
        }

        public string Server
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Target
        {
            get
            {
                throw new NotImplementedException();
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
                Server = null,
                Sender = this.Sender,
            };
        }
    }
}