using Homero.Messages;

namespace Homero.EventArgs
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
