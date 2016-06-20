using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Client;

namespace homeronet.Messages
{
    public interface IStandardMessage
    {
        string Message { get; }
        string Sender { get; }
        string Target { get; }
        string Channel { get; }
        string Server { get; }
        bool IsPrivate { get; }
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
