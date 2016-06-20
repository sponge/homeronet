using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Messages;
using SuperSocket.ClientEngine.Protocol;

namespace homeronet.EventArgs
{
    public class CommandReceivedEventArgs : System.EventArgs
    {
        public ITextCommand Command;

        public CommandReceivedEventArgs(ITextCommand command)
        {
            Command = command;
        }
    }
}
