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
        string User { get; }
        string Channel { get; }
        bool IsPrivate { get; }
    }
}
