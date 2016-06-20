using System;
using System.Collections.Generic;
using Homero.Messages.Attachments;

namespace Homero.Messages
{
    public interface IStandardMessage
    {
        string Message { get; }
        string Sender { get; }
        string Target { get; }
        string Channel { get; }
        string Server { get; }
        bool IsPrivate { get; }
        List<IAttachment> Attachments { get; } 
        IStandardMessage CreateResponse(string message = null);
    }

    public class StandardMessage : IStandardMessage
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Target { get; set; }
        public string Channel { get; set; }
        public string Server { get; set; }
        public bool IsPrivate { get; set; }
        public List<IAttachment> Attachments { get; set; }

        public StandardMessage()
        {
            Attachments = new List<IAttachment>();
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
