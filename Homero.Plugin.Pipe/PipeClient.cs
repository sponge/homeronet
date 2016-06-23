using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Messages.Attachments;
using Homero.Services;

namespace Homero.Plugin.Pipe
{
    /// <exclude />
    public class PipeClient : IClient
    {
        private IClient _innerClient;
        private MessageBrokerService _messageBroker;
        public List<string> _commandChain = new List<string>();
        private ITextCommand _innerCommand;
        public PipeClient(IClient innerClient, IMessageBroker broker, ITextCommand innerCommand)
        {
            _innerCommand = innerCommand;
            _innerClient = innerClient;
            _messageBroker = broker as MessageBrokerService; // oops im breaking IoC for this, todo: expose invocation over standard interface
        }
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

        public void FireFirstMessage(ITextCommand command)
        {
            _messageBroker.RaiseCommandReceived(this, command);
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
            if (_commandChain.Count == 0)
            {
                _innerClient.ReplyTo(_innerCommand, reply);
            }
            else
            {
                StandardMessage curMessage = new StandardMessage()
                {
                    /*
                    Attachments = originalCommand.InnerMessage.Attachments,
                    Channel = originalCommand.InnerMessage.Channel,
                    IsPrivate = originalCommand.InnerMessage.IsPrivate,
                    Message = reply,
                    Sender = originalCommand.InnerMessage.Sender,
                    Server = originalCommand.InnerMessage.Server,
                    Target = originalCommand.InnerMessage.Target
                    */
                };
                
                TextCommand newCommand = new TextCommand();
                newCommand.InnerMessage = curMessage;
                newCommand.Command = _commandChain.First();
                newCommand.Arguments = reply.Split(' ').ToList();
                _commandChain.RemoveAt(0);
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

        public bool IsConnected { get { return true; } }
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageSentEventArgs> MessageSent;
    }
}
