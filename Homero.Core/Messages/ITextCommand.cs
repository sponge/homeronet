using Homero.Core.Messages.Attachments;
using Homero.Core.Properties;
using System.Collections.Generic;
using System.Linq;

namespace Homero.Core.Messages
{
    public interface ITextCommand
    {
        string Command { get; }

        List<string> Arguments { get; }

        List<IAttachment> Attachments { get; }
    }

    public class TextCommand : ITextCommand
    {
        public TextCommand()
        {
        }

        public TextCommand(IStandardMessage message)
        {
            var splitMsg = message.Message.Split(' ');
            Command = splitMsg[0].TrimStart(Settings.Default.CommandPrefix.ToCharArray()).Trim('\n', ' ');
            Arguments = splitMsg.Length > 1 ? splitMsg.Skip(1).ToList() : new List<string>();
            Attachments = message.Attachments;
        }

        public string Command { get; set; }

        public List<string> Arguments { get; set; }

        public List<IAttachment> Attachments { get; set; }
    }
}