using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Properties;
using Homero.Core.Utility;
using Homero.Plugin;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using Speedy.Linq;

namespace Homero.Core.Services
{
    public class MessageBrokerService : IMessageBroker
    {
        private readonly WeakEventSource<CommandReceivedEventArgs> _commandEventSource =
            new WeakEventSource<CommandReceivedEventArgs>();

        private readonly WeakEventSource<CommandSieveEventArgs> _commandSieveEventSource =
            new WeakEventSource<CommandSieveEventArgs>();

        private readonly WeakEventSource<MessageEventArgs> _messageReceivedEventSource =
            new WeakEventSource<MessageEventArgs>();

        private readonly WeakEventSource<MessageEventArgs> _messageSentEventSource =
            new WeakEventSource<MessageEventArgs>();

        private readonly WeakEventSource<CommandReceivedEventArgs> _commandDispatchingEventSource =
            new WeakEventSource<CommandReceivedEventArgs>();
        private readonly WeakEventSource<CommandReceivedEventArgs> _commandDispatchedEventSource =
            new WeakEventSource<CommandReceivedEventArgs>();
        private readonly WeakEventSource<EventFailedEventArgs> _commandFailedEventSource =
                    new WeakEventSource<EventFailedEventArgs>();



        private IKernel _kernel;
        public MessageBrokerService(IKernel kernel)
        {
            _kernel = kernel;

            foreach (var client in kernel.GetAll<IClient>())
            {
                client.MessageReceived += ClientOnMessageReceived;
                client.MessageSent += ClientOnMessageSent;
            }
            _commandEventSource.AsyncRaiseFailed += CommandEventSourceOnAsyncRaiseFailed;
            _commandEventSource.InvokeStarted += CommandEventSourceOnInvokeStarted;
            _commandEventSource.InvokeEnded += CommandEventSourceOnInvokeEnded;
        }

        private void CommandEventSourceOnInvokeEnded(object sender, System.EventArgs eventArgs)
        {
            _commandDispatchedEventSource.RaiseAsync(sender, eventArgs as CommandReceivedEventArgs);
        }

        private void CommandEventSourceOnInvokeStarted(object sender, System.EventArgs eventArgs)
        {
            _commandDispatchingEventSource.RaiseAsync(sender, eventArgs as CommandReceivedEventArgs);
        }

        private void CommandEventSourceOnAsyncRaiseFailed(object sender, EventFailedEventArgs e)
        {
            if (e.OriginalEventArgs is CommandReceivedEventArgs)
            {
                _commandFailedEventSource.RaiseAsync(sender, e);
            }
        }

        public event EventHandler<CommandReceivedEventArgs> CommandDispatched
        {
            add
            {
                _commandDispatchedEventSource.Subscribe(value);
            }
            remove
            {
                _commandDispatchedEventSource.Unsubscribe(value);
            }
        }

        public event EventHandler<CommandReceivedEventArgs> CommandDispatching
        {
            add
            {
                _commandDispatchingEventSource.Subscribe(value);
            }
            remove
            {
                _commandDispatchingEventSource.Unsubscribe(value);
            }
        }
        public event EventHandler<EventFailedEventArgs> CommandFailed
        {
            add
            {
                _commandFailedEventSource.Subscribe(value);
            }
            remove
            {
                _commandFailedEventSource.Unsubscribe(value);
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

        public event EventHandler<MessageEventArgs> MessageReceived
        {
            add { _messageReceivedEventSource.Subscribe(value); }
            remove { _messageReceivedEventSource.Unsubscribe(value); }
        }

        public event EventHandler<MessageEventArgs> MessageSent
        {
            add { _messageSentEventSource.Subscribe(value); }
            remove { _messageSentEventSource.Unsubscribe(value); }
        }

        public void ClientOnMessageSent(object sender, MessageEventArgs e)
        {
            _messageSentEventSource.RaiseAsync(sender, e);
        }

        public void ClientOnMessageReceived(object sender, MessageEventArgs e)
        {
            // Determine if plugin first.
            if (e.Message.Message.StartsWith(Settings.Default.CommandPrefix))
            {
                var command = new TextCommand(e.Message);
                RaiseCommandReceived(sender, command, e);
                return;
            }

            _messageReceivedEventSource.Raise(sender, e);
        }

        public void RaiseCommandReceived(object sender, ITextCommand command, MessageEventArgs e)
        {
            // First check all plugins and see if any command comes close.
            List<string> potentialCommands = (from plugin in _kernel.GetAll<IPlugin>().Where(plugin => plugin.RegisteredTextCommands != null) from cmd in plugin.RegisteredTextCommands where cmd.StartsWith(command.Command) select cmd).ToList();

            if (!potentialCommands.Contains(command.Command))
            {
                if (potentialCommands.Count > 1)
                {
                    e.ReplyTarget.Send("Did you mean: " + string.Join(", ", potentialCommands));
                    return;
                }
                else if (potentialCommands.Count == 1)
                {
                    ((TextCommand)command).Command = potentialCommands.First(); // force override i don't give a heck
                }
                else
                {
                    e.ReplyTarget.Send("the fuck is that");
                    return;
                }
            }

            _commandEventSource.RaiseAsync(sender, new CommandReceivedEventArgs(command, e.Server, e.Channel, e.User), delegate (object o)
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