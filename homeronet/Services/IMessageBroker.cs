using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.EventArgs;
using homeronet.Messages;

namespace homeronet.Services
{
    public interface IMessageBroker
    {
        event EventHandler<CommandReceivedEventArgs> CommandReceived;
        event EventHandler<CommandSieveEventArgs> CommandSieving; // Feels weird to have this in the message broker.

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<MessageSentEventArgs> MessageSent;

    }
}
