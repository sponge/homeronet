using Homero.Messages;
using Homero.Plugin;

namespace Homero.EventArgs
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
