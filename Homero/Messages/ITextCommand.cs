using System.Collections.Generic;
using System.Linq;
using Homero.Properties;

namespace Homero.Messages
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

        public TextCommand()
        {
            
        }

        public TextCommand(IStandardMessage message)
        {
            InnerMessage = message;
            string[] splitMsg = InnerMessage.Message.Split(' ');
            Command = splitMsg[0].TrimStart(Settings.Default.CommandPrefix.ToCharArray());
            Arguments = splitMsg.Length > 1 ? splitMsg.Skip(1).ToList() : new List<string>();
        }
    }
}
