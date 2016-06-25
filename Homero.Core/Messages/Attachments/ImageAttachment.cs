using System;
using System.IO;
using RestSharp.Extensions;

namespace Homero.Core.Messages.Attachments
{
    public class ImageAttachment : IAttachment
    {
        private byte[] _data;
        private Stream _stream;

        public ImageAttachment()
        {
        }

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
                return _data;
                ;
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


        public string Name { get; set; }

        public Uri Uri { get; set; }
    }
}