using System;
using Homero.EventArgs;

namespace Homero.Services
{
    public interface IMessageBroker
    {
        event EventHandler<CommandReceivedEventArgs> CommandReceived;
        event EventHandler<CommandSieveEventArgs> CommandSieving; // Feels weird to have this in the message broker.

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<MessageSentEventArgs> MessageSent;

    }
}
