using System;
using System.IO;

namespace Homero.Core.Messages.Attachments
{
    public class FileAttachment : IAttachment
    {
        public byte[] Data { get; }

        public Stream DataStream { get; }

        public string Name { get; set; }

        public Uri Uri { get; set; }
    }
}