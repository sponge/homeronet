using Homero.Core.Messages;

namespace Homero.Core.EventArgs
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