using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Goon
{
    internal class FortuneFile
    {
        public String Command { get; set; }
        public String Path { get; set;  }
        public bool StripNewLines { get; set; }
        public bool IsMultiLine { get; set; }

        public FortuneFile(string command, string path, bool stripNewLines, bool isMultiline)
        {
            Command = command;
            Path = path;
            StripNewLines = stripNewLines;
            IsMultiLine = isMultiline;
        }
    }
}
