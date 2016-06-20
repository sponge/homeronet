using System;
using System.IO;

namespace Homero.Messages.Attachments
{
    public class FileAttachment : IAttachment
    {
        private byte[] _data;
        private Stream _dataStream;
        private string _name;
        private Uri _uri;

        public byte[] Data
        {
            get { return _data; }
        }

        public Stream DataStream
        {
            get { return _dataStream; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Uri Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }
    }
}
