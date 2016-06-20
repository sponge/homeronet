﻿using homeronet.Client;
using homeronet.EventArgs;
using homeronet.Messages;
using homeronet.Properties;
using Ninject;
using System;
using WeakEvent;

namespace homeronet.Services
{
    public class MessageBrokerService : IMessageBroker
    {
        private readonly WeakEventSource<CommandReceivedEventArgs> _commandEventSource = new WeakEventSource<CommandReceivedEventArgs>();
        private readonly WeakEventSource<MessageSentEventArgs> _messageSentEventSource = new WeakEventSource<MessageSentEventArgs>();
        private readonly WeakEventSource<MessageReceivedEventArgs> _messageReceivedEventSource = new WeakEventSource<MessageReceivedEventArgs>();

        public event EventHandler<CommandReceivedEventArgs> CommandReceived
        {
            add { _commandEventSource.Subscribe(value); }
            remove { _commandEventSource.Unsubscribe(value); }
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

        public MessageBrokerService(IKernel kernel)
        {
            foreach (IClient client in kernel.GetAll<IClient>())
            {
                client.MessageReceived += ClientOnMessageReceived;
                client.MessageSent += ClientOnMessageSent;
            }
        }

        private void ClientOnMessageSent(object sender, MessageSentEventArgs e)
        {
            _messageSentEventSource.RaiseAsync(sender, e);
        }

        private void ClientOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Determine if plugin first.
            if (e.Message.Message.StartsWith(Settings.Default.CommandPrefix))
            {
                TextCommand command = new TextCommand(e.Message);
                _commandEventSource.RaiseAsync(sender, new CommandReceivedEventArgs(command));
                return;
            }

            _messageReceivedEventSource.RaiseAsync(sender, e);
        }
    }
}