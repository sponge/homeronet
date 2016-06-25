using Homero.Core.Messages;
using Homero.Plugin;

namespace Homero.Core.EventArgs
{
    public class CommandSieveEventArgs
    {
        public ITextCommand Command;
        public IPlugin Target;

        public CommandSieveEventArgs(IPlugin target, ITextCommand command)
        {
            Pass = true;
            Target = target;
            Command = command;
        }

        public bool Pass { get; set; }
    }
}