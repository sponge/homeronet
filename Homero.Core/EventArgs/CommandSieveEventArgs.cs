using Homero.Core.Messages;

namespace Homero.Core.EventArgs
{
    public class CommandSieveEventArgs
    {
        public ITextCommand Command;
        public IPlugin TargetPlugin;

        public IServer Server { get; }
        public IChannel Channel { get; }
        public IUser User { get; }

        public ISendable Target
        {
            get
            {
                if (Channel != null)
                {
                    return Channel;
                }
                return User;
            }
        }

        public CommandSieveEventArgs(IPlugin target, ITextCommand command)
        {
            Pass = true;
            TargetPlugin = target;
            Command = command;
        }

        public bool Pass { get; set; }
    }
}