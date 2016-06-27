using System.Collections.Generic;
using System.Linq;
using Homero.Core.Properties;

namespace Homero.Core.Messages
{
    public interface ITextCommand
    {
        IStandardMessage InnerMessage { get; }
        string Command { get; }
        List<string> Arguments { get; }
    }

    public class TextCommand : ITextCommand
    {
        public TextCommand()
        {
        }

        public TextCommand(IStandardMessage message)
        {
            InnerMessage = message;
            var splitMsg = InnerMessage.Message.Split(' ');
            Command = splitMsg[0].TrimStart(Settings.Default.CommandPrefix.ToCharArray()).Trim('\n', ' ');
            Arguments = splitMsg.Length > 1 ? splitMsg.Skip(1).ToList() : new List<string>();
        }

        public IStandardMessage InnerMessage { get; set; }
        public string Command { get; set; }
        public List<string> Arguments { get; set; }
    }
}