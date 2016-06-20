using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Messages.Attachments
{
    public interface IAttachment
    {
        byte[] Data { get; }
        Stream DataStream { get; }
        string Name { get; }
        Uri Uri { get; }
    }
}
