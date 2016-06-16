using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    class StandardMessage : IStandardMessage
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Target { get; set; }
        public string Channel { get; set; }
        public string Server { get; set; }
        public bool IsPrivate { get; set; }
    }
}
