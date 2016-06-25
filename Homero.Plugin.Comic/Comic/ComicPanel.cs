using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Comic
{
    public class ComicPanel
    {
        public List<ComicMessage> Messages { get; set; }

        public ComicPanel()
        {
            Messages = new List<ComicMessage>();
        }
    }

    public class ComicMessage
    {
        public string User { get; set; }
        public string Message { get; set; }
    }
}
