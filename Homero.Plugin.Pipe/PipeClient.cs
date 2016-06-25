using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;

namespace Homero.Plugin.Pipe
{
    /// <exclude />
    public class PipeClient : IClient
    {
        private IClient _innerClient;
        private ITextCommand _innerCommand;
        private MessageBrokerService _messageBroker;

        public PipeClient(IClient innerClient, IMessageBroker broker, ITextCommand innerCommand)
        {
            _innerCommand = innerCommand;
            _innerClient = innerClient;
            _messageBroker = broker as MessageBrokerService;
                // oops im breaking IoC for this, todo: expose invocation over standard interface
        }

        public List<string> CommandChain { get; set; }

        public string Name
        {
            get { return "Pipe Client"; }
        }

        public string Description
        {
            get { return "A mock client to handle plugins jibber jabbering back and forth."; }
        }

        public Version Version { get; }

        public bool MarkdownSupported
        {
            get { return _innerClient.MarkdownSupported; }
        }

        public bool AudioSupported
        {
            get { return _innerClient.AudioSupported; }
        }

        public bool IrcFormattingSupported
        {
            get { return _innerClient.IrcFormattingSupported; }
        }

        public bool InlineOrOembedSupported
        {
            get { return _innerClient.InlineOrOembedSupported; }
        }

        public Task<bool> Connect()
        {
            return null;
        }

        public Task DispatchMessage(IStandardMessage message)
        {
            return null;
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply)
        {
            // pignored
        }

        public void ReplyTo(IStandardMessage originalMessage, IAttachment attachment)
        {
            // dignored
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, IAttachment attachment)
        {
            // engored
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, List<IAttachment> attachments)
        {
            // ig-ghored
        }

        public void ReplyTo(IStandardMessage originalMessage, IStandardMessage reply)
        {
            // ignored u think i give a care?
        }

        public void ReplyTo(ITextCommand originalCommand, string reply)
        {
            if (CommandChain.Count == 0)
            {
                _innerClient.ReplyTo(_innerCommand, reply);
            }
            else
            {
                var newCommand = new TextCommand();
                newCommand.Command = CommandChain.First();
                newCommand.Arguments = reply.Split(' ').ToList();
                CommandChain.RemoveAt(0);
                _messageBroker.RaiseCommandReceived(this, newCommand);
            }
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, IAttachment attachment)
        {
            // ig a nore
        }

        public void ReplyTo(ITextCommand originalCommand, IAttachment attachment)
        {
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, List<IAttachment> attachments)
        {
        }

        public void ReplyTo(ITextCommand originalCommand, IStandardMessage reply)
        {
            // u also get ignored
        }

        public bool IsConnected
        {
            get { return true; }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageSentEventArgs> MessageSent;

        public void FireFirstMessage(ITextCommand command)
        {
            _messageBroker.RaiseCommandReceived(this, command);
        }
    }
}