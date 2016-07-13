using Homero.Core.EventArgs;
using Homero.Core.Interface;
using Homero.Core.Messages;
using Homero.Core.Messages.Attachments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homero.Core.Client
{
    public interface IClient
    {
        string Name { get; }

        string Description { get; }

        Version Version { get; }

        bool MarkdownSupported { get; }

        bool AudioSupported { get; }

        bool IrcFormattingSupported { get; }

        bool InlineOrOembedSupported { get; }

        bool IsConnected { get; }

        Task<bool> Connect();

        List<IServer> Servers { get; }

        event EventHandler<MessageEventArgs> MessageReceived;

        event EventHandler<MessageEventArgs> MessageSent;
    }
}