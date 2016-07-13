using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Messages.Attachments;

namespace Homero.Core.Interface
{
    public interface ISendable
    {
        void Send(string Message);
        void Send(string Message, params object[] Format);
        void Send(string Message, params IAttachment[] Attachments);
    }
}
