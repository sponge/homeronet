using System;
using System.IO;

namespace Homero.Messages.Attachments
{
    public interface IAttachment
    {
        byte[] Data { get; }
        Stream DataStream { get; }
        string Name { get; }
        Uri Uri { get; }
    }
}
