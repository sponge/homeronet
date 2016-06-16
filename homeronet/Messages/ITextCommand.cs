using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Messages
{
    public interface ITextCommand
    {
        IStandardMessage InnerMessage { get; }
        string Command { get; }
        List<string> Arguments { get; }
    }

    class TextCommand : ITextCommand
    {
        public IStandardMessage InnerMessage { get; set; }
        public string Command { get; set; }
        public List<string> Arguments { get; set; }
    }
}
