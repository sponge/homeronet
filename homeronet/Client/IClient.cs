using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using homeronet.EventArgs;
using homeronet.Messages;
using homeronet.Messages.Attachments;

namespace homeronet.Client
{
    public interface IClient
    {
        string Name { get; }
        string Description { get; }
        Version Version { get; }
        bool MarkdownSupported { get; }
        bool AudioSupported { get; }
        bool IrcFormattingSupported { get; }
        Task<bool> Connect();
        Task DispatchMessage(IStandardMessage message);
        void ReplyTo(IStandardMessage originalMessage, string reply);
        void ReplyTo(IStandardMessage originalMessage, IAttachment attachment);
        void ReplyTo(IStandardMessage originalMessage, string reply, IAttachment attachment);
        void ReplyTo(IStandardMessage originalMessage, string reply, List<IAttachment> attachments);

        void ReplyTo(IStandardMessage originalMessage, IStandardMessage reply);
        void ReplyTo(ITextCommand originalCommand, string reply);
        void ReplyTo(ITextCommand originalCommand, string reply, IAttachment attachment);
        void ReplyTo(ITextCommand originalCommand, IAttachment attachment);
        void ReplyTo(ITextCommand originalCommand, string reply, List<IAttachment> attachments);

        void ReplyTo(ITextCommand originalCommand, IStandardMessage reply);
        
        bool IsConnected { get; }

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<MessageSentEventArgs> MessageSent;
    }
}
