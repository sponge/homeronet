using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero
{
    public interface IAttachment
    {
        /// <summary>
        /// The name of the attachment, generally a filename.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The attachment contents.
        /// </summary>
        object Contents { get; }
    }
}