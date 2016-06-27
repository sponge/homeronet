using System.Collections.Generic;
using Homero.Core.Messages.Attachments;

namespace Homero.Core.Messages
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
        public StandardMessage()
        {
            Attachments = new List<IAttachment>();
        }

        public string Message { get; set; }
        public string Sender { get; set; }
        public string Target { get; set; }
        public string Channel { get; set; }
        public string Server { get; set; }
        public bool IsPrivate { get; set; }
        public List<IAttachment> Attachments { get; set; }

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