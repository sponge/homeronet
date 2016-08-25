using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Core.Messages.Attachments
{
    public class TextAttachment : IAttachment
    {
        private byte[] _data;
        private Stream _stream;

        public TextAttachment(string text)
        {
            _data = Encoding.UTF8.GetBytes(text);
            _stream = new MemoryStream(_data);
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }
            set { _data = value; }
        }

        public Stream DataStream
        {
            get
            {
                return _stream;
            }
            set { _stream = value;  }
        }

        public string Name { get; set; }

        public Uri Uri { get; set; }
    }
}
