using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Homero
{
    public interface IMessage
    {
        /// <summary>
        /// The message text contents.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// A collection of all attachments in the message. List order is equal to attachment order.
        /// </summary>
        List<IAttachment> Attachments { get; }
    }
}