using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using IrcDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Homero.Core.Messages;
using Homero.Core.Messages.Attachments;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using Homero.Messages;
using Homero.Core.Utility;

namespace Homero.Core.Client
{
    public class IrcClient : IClient
    {
        private StandardIrcClient _ircClient;
        private IConfiguration _config;
        private ILogger _logger;
        private IUploader _uploader;
        private bool _isConnected = false;

        public IrcClient(IConfiguration config, ILogger logger, IUploader uploader)
        {
            _config = config;
            _logger = logger;
            _uploader = uploader;
            _ircClient = new StandardIrcClient();

            _ircClient.Registered += IrcClientOnConnectionComplete;
            _ircClient.RawMessageSent += (s, e) => _logger.Info(e.RawContent);
            _ircClient.RawMessageReceived += (s, e) => _logger.Info(e.RawContent);
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageSentEventArgs> MessageSent;

        private void IrcClientOnConnectionComplete(object sender, System.EventArgs eventArgs)
        {
            // wrapped in a closure because we can't get at the sending channel from the sender and eventargs.
            _ircClient.LocalUser.JoinedChannel += (s, e) =>
            {
                e.Channel.MessageReceived += (ss, ee) => IrcClientOnMessageReceived(ss, ee, e.Channel);
            };

            foreach(string channel in _config.GetValue<List<string>>("channels"))
            {
                _ircClient.Channels.Join(channel);
            }
        }

        private void IrcClientOnMessageReceived(object sender, IrcMessageEventArgs eventArgs, IrcChannel channel)
        {
            if (eventArgs.Source.Name == _ircClient.LocalUser.NickName)
            {
                return;
            }
            IrcMessage message = new IrcMessage(this, eventArgs, channel);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        public bool AudioSupported => false;
        public string Description => "IRC Client using IrcDotNet";
        public bool InlineOrOembedSupported => false;
        public bool IrcFormattingSupported => true;
        public bool MarkdownSupported => false;
        public string Name => "IRC Client";
        public Version Version => new Version(0, 0, 1);

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public Task<bool> Connect()
        {
            // don't think anyone will actually care about having a different nick/user/real, right?
            _ircClient.Connect(new Uri("irc://" + _config.GetValue<string>("server")), new IrcUserRegistrationInfo
            {
                NickName = _config.GetValue<string>("name"),
                UserName = _config.GetValue<string>("name"),
                RealName = _config.GetValue<string>("name"),
            });
            return new Task<bool>(() => true);
        }

        public async Task DispatchMessage(IStandardMessage message)
        {
            IrcChannel channel = _ircClient.Channels.FirstOrDefault(x => x.Name == message.Channel);
            List<string> messagesToSend = new List<string>();

            // split up multi-line messages
            messagesToSend.AddRange(message.Message.Split('\n'));

            // upload each attachment.
            foreach (IAttachment attachment in message.Attachments)
            {
                if (attachment is ImageAttachment)
                {
                    string image = await _uploader.Upload(attachment as ImageAttachment);
                    messagesToSend.Add(image);
                }
            }

            foreach (string msg in messagesToSend)
            {
                _ircClient.LocalUser.SendMessage(channel, msg);
            }

            MessageSent?.Invoke(this, new MessageSentEventArgs(message));
        }

        public void ReplyTo(IStandardMessage originalMessage, IStandardMessage reply)
        {
            DispatchMessage(reply);
        }

        public void ReplyTo(ITextCommand originalCommand, string reply)
        {
            ReplyTo(originalCommand, originalCommand.InnerMessage.CreateResponse(reply));
        }

        public void ReplyTo(ITextCommand originalCommand, IAttachment attachment)
        {
            ReplyTo(originalCommand, String.Empty, attachment);
        }

        public void ReplyTo(IStandardMessage originalMessage, IAttachment attachment)
        {
            ReplyTo(originalMessage, String.Empty, attachment);
        }

        public void ReplyTo(ITextCommand originalCommand, IStandardMessage reply)
        {
            DispatchMessage(reply);
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply)
        {
            ReplyTo(originalMessage, originalMessage.CreateResponse(reply));
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, List<IAttachment> attachments)
        {
            ReplyTo(originalCommand.InnerMessage, reply, attachments);
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, IAttachment attachment)
        {
            ReplyTo(originalCommand.InnerMessage, reply, attachment);
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, List<IAttachment> attachments)
        {
            IStandardMessage message = originalMessage.CreateResponse(reply);
            message.Attachments.AddRange(attachments);
            DispatchMessage(message);
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, IAttachment attachment)
        {
            IStandardMessage message = originalMessage.CreateResponse(reply);
            message.Attachments.Add(attachment);
            ReplyTo(originalMessage, message);
        }
    }
}
