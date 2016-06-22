using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Messages.Attachments;
using Homero.Services;

namespace Homero.Client
{
    public class DiscordClient : IClient
    {
        private Discord.DiscordClient _discordClient;
        private IConfiguration _config;

        #region Constructors

        public DiscordClient(IConfiguration config)
        {
            _config = config;

            if (String.IsNullOrEmpty(_config.GetValue<string>("key")))
            {
                _config.SetValue("key", "THISISNOTAREALAPIKEY");
                throw new Exception("No API key specified. Created a default file for editing.");
            }

            _discordClient = new Discord.DiscordClient(x =>
            {
                // Seriously, a configuration constructor in an action?
                x.AppName = "Homero.NET";
                x.AppUrl = "https://goodass.dog";
            });

            _discordClient.MessageReceived += DiscordClientOnMessageReceived;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler<MessageSentEventArgs> MessageSent;

        #endregion Events

        #region Async Methods

        public async Task<bool> Connect()
        {
            await _discordClient.Connect(_config.GetValue<string>("key"));
            return true; // uh why can't i get the connect result?
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply)
        {
            ReplyTo(originalMessage, originalMessage.CreateResponse(reply));
                // TODO: Remove CreateResponse and remove from Interface.
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, IAttachment attachment)
        {
            IStandardMessage msg = originalMessage.CreateResponse(reply);
            msg.Attachments.Add(attachment);
            ReplyTo(originalMessage, msg);
        }

        public void ReplyTo(IStandardMessage originalMessage, IAttachment attachment)
        {
            ReplyTo(originalMessage, String.Empty, attachment);
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, List<IAttachment> attachments)
        {
            IStandardMessage msg = originalMessage.CreateResponse(reply);
            msg.Attachments.AddRange(attachments);
            ReplyTo(originalMessage, msg);
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

        public void ReplyTo(ITextCommand originalCommand, string reply, IAttachment attachment)
        {
            IStandardMessage msg = originalCommand.InnerMessage.CreateResponse(reply);
            msg.Attachments.Add(attachment);
            ReplyTo(originalCommand, msg);
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, List<IAttachment> attachments)
        {
            IStandardMessage msg = originalCommand.InnerMessage.CreateResponse(reply);
            msg.Attachments.AddRange(attachments);
            ReplyTo(originalCommand, msg);
        }

        public void ReplyTo(ITextCommand originalCommand, IStandardMessage reply)
        {
            DispatchMessage(reply);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public async Task DispatchMessage(IStandardMessage message)
        {
            Discord.Channel targetChannel = null;

            // Is it a PM or a public message?
            if (message.IsPrivate)
            {
                targetChannel = _discordClient.PrivateChannels.FirstOrDefault(x => x.Name == message.Channel);
            }
            else
            {
                targetChannel = (_discordClient.Servers
                    .FirstOrDefault(x => x.Name == message.Server)?.TextChannels)?.FirstOrDefault(
                        x => x.Name == message.Channel);
            }

            bool sentAttachment = false;

            foreach (IAttachment attachment in message.Attachments)
            {
                await targetChannel?.SendFile(attachment.Name, attachment.DataStream);
                sentAttachment = true;
            }

            if (String.IsNullOrEmpty(message.Message) && sentAttachment)
            {
                return;
            }

            var sendMessage = targetChannel?.SendMessage(message.Message);
            if (sendMessage != null)
            {
                await sendMessage;
                MessageSent?.Invoke(this, new MessageSentEventArgs(message));
            }
        }

        #endregion Async Methods

        #region Methods

        private void DiscordClientOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            if (messageEventArgs.User.Id == _discordClient.CurrentUser.Id)
            {
                return;
            }
            DiscordMessage message = new DiscordMessage(this, messageEventArgs.Message);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        #endregion Methods

        #region Properties

        public bool IsConnected
        {
            get { return _discordClient.State == ConnectionState.Connected; }
        }

        public string Name => "Discord.NET Client";

        public string Description => "Client that connects to Discord using the Discord.NET library.";

        public Version Version => new Version(0, 0, 1);
        public bool MarkdownSupported => true;
        public bool AudioSupported => true;
        public bool IrcFormattingSupported => false;
        public bool InlineOrOembedSupported => true;

        public Discord.DiscordClient RootClient
        {
            get { return _discordClient; }
        }

        #endregion Properties
    }
}