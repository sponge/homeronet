using RestSharp.Extensions;
using System;
using System.IO;

namespace homeronet.Messages.Attachments
{
    public class ImageAttachment : IAttachment
    {
        private string _name;
        private Uri _uri;
        private Stream _stream;
        private byte[] _data;

        public ImageAttachment(string FilePath)
        {
            Name = Path.GetFileName(FilePath);
            DataStream = File.OpenRead(FilePath);
        }

        public byte[] Data
        {
            get
            {
                if (_data == null && DataStream != null)
                {
                    return DataStream.ReadAsBytes(); // TODO: Not restsharp extension.
                }
                return _data; ;
            }
            set { _data = value; }
        }

        public Stream DataStream
        {
            get
            {
                if (_stream == null && Data != null)
                {
                    _stream = new MemoryStream(Data); // TODO: Dispose
                }

                return _stream;
            }
            set { _stream = value; }
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