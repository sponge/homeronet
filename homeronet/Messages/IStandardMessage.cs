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
}
