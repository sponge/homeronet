using System;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Properties;
using Homero.Core.Utility;
using Homero.Plugin;
using Ninject;

namespace Homero.Core.Services
{
    public class MessageBrokerService : IMessageBroker
    {
        private readonly WeakEventSource<CommandReceivedEventArgs> _commandEventSource =
            new WeakEventSource<CommandReceivedEventArgs>();

        private readonly WeakEventSource<CommandSieveEventArgs> _commandSieveEventSource =
            new WeakEventSource<CommandSieveEventArgs>();

        private readonly WeakEventSource<MessageReceivedEventArgs> _messageReceivedEventSource =
            new WeakEventSource<MessageReceivedEventArgs>();

        private readonly WeakEventSource<MessageSentEventArgs> _messageSentEventSource =
            new WeakEventSource<MessageSentEventArgs>();

        public MessageBrokerService(IKernel kernel)
        {
            foreach (var client in kernel.GetAll<IClient>())
            {
                client.MessageReceived += ClientOnMessageReceived;
                client.MessageSent += ClientOnMessageSent;
            }
        }

        public event EventHandler<CommandReceivedEventArgs> CommandReceived
        {
            add { _commandEventSource.Subscribe(value); }
            remove { _commandEventSource.Unsubscribe(value); }
        }

        public event EventHandler<CommandSieveEventArgs> CommandSieving
        {
            add { _commandSieveEventSource.Subscribe(value); }
            remove { _commandSieveEventSource.Unsubscribe(value); }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add { _messageReceivedEventSource.Subscribe(value); }
            remove { _messageReceivedEventSource.Unsubscribe(value); }
        }

        public event EventHandler<MessageSentEventArgs> MessageSent
        {
            add { _messageSentEventSource.Subscribe(value); }
            remove { _messageSentEventSource.Unsubscribe(value); }
        }

        public void ClientOnMessageSent(object sender, MessageSentEventArgs e)
        {
            _messageSentEventSource.RaiseAsync(sender, e);
        }

        public void ClientOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Determine if plugin first.
            if (e.Message.Message.StartsWith(Settings.Default.CommandPrefix))
            {
                var command = new TextCommand(e.Message);
                RaiseCommandReceived(sender, command);
                return;
            }

            _messageReceivedEventSource.Raise(sender, e);
        }

        public void RaiseCommandReceived(object sender, ITextCommand command)
        {
            _commandEventSource.RaiseAsync(sender, new CommandReceivedEventArgs(command), delegate(object o)
            {
                var pluginInstance = o as IPlugin;
                if (pluginInstance?.RegisteredTextCommands?.Contains(command.Command) == true)
                {
                    // Run Sieve.
                    var sieveEvent = new CommandSieveEventArgs(pluginInstance, command);
                    _commandSieveEventSource.Raise(this, sieveEvent);
                    return sieveEvent.Pass;
                }
                return false;
            });
        }
    }
}