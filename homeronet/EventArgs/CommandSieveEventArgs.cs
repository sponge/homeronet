using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Messages;
using homeronet.Plugin;

namespace homeronet.EventArgs
{
    public class CommandSieveEventArgs
    {
        public IPlugin Target;
        public ITextCommand Command;
        public bool Pass { get; set; }

        public CommandSieveEventArgs(IPlugin target, ITextCommand command)
        {
            Pass = true;
            Target = target;
            Command = command;
        }
    }
}
