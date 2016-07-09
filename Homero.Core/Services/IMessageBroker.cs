using Homero.Core.EventArgs;
using System;

namespace Homero.Core.Services
{
    public interface IMessageBroker
    {
        event EventHandler<CommandReceivedEventArgs> CommandReceived;

        event EventHandler<CommandSieveEventArgs> CommandSieving; // Feels weird to have this in the message broker.

        event EventHandler<MessageEventArgs> MessageReceived;

        event EventHandler<MessageEventArgs> MessageSent;
    }
}