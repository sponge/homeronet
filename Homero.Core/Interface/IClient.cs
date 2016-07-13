using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Core.EventArgs;

namespace Homero.Core.Interface
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