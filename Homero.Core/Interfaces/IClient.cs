using Homero.Core.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homero.Core
{
    public interface IClient
    {
        string Name { get; }

        string Description { get; }

        Version Version { get; }

        ClientFeature Features { get; }

        bool IsConnected { get; }

        Task<bool> Connect();

        List<IServer> Servers { get; }

        event EventHandler<MessageEventArgs> MessageReceived;

        event EventHandler<MessageEventArgs> MessageSent;
    }
}