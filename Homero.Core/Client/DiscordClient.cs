using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;

namespace Homero.Core.Client
{
    public class DiscordClient : IClient
    {
        private IConfiguration _config;

        #region Constructors

        public DiscordClient(IConfiguration config)
        {
            _config = config;

            if (string.IsNullOrEmpty(_config.GetValue<string>("key")))
            {
                _config.SetValue("key", "THISISNOTAREALAPIKEY");
                throw new Exception("No API key specified. Created a default file for editing.");
            }

            RootClient = new Discord.DiscordClient(x =>
            {
                // Seriously, a configuration constructor in an action?
                x.AppName = "Homero.NET";
                x.AppUrl = "https://goodass.dog";
            });

            RootClient.MessageReceived += DiscordClientOnMessageReceived;
        }

        #endregion Constructors

        #region Methods

        private void DiscordClientOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            if (messageEventArgs.User.Id == RootClient.CurrentUser.Id)
            {
                return;
            }
            var message = new DiscordMessage(this, messageEventArgs.Message);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        #endregion Methods

        #region Events

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler<MessageSentEventArgs> MessageSent;

        #endregion Events

        #region Async Methods

        public async Task<bool> Connect()
        {
            await RootClient.Connect(_config.GetValue<string>("key"));
            return true; // uh why can't i get the connect result?
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply)
        {
            ReplyTo(originalMessage, originalMessage.CreateResponse(reply));
            // TODO: Remove CreateResponse and remove from Interface.
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, IAttachment attachment)
        {
            var msg = originalMessage.CreateResponse(reply);
            msg.Attachments.Add(attachment);
            ReplyTo(originalMessage, msg);
        }

        public void ReplyTo(IStandardMessage originalMessage, IAttachment attachment)
        {
            ReplyTo(originalMessage, string.Empty, attachment);
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, List<IAttachment> attachments)
        {
            var msg = originalMessage.CreateResponse(reply);
            msg.Attachments.AddRange(attachments);
            ReplyTo(originalMessage, msg);
        }

        public void ReplyTo(IStandardMessage originalMessage, IStandardMessage reply)
        {
#pragma warning disable 4014
            DispatchMessage(reply);
#pragma warning restore 4014
        }

        public void ReplyTo(ITextCommand originalCommand, string reply)
        {
            ReplyTo(originalCommand, originalCommand.InnerMessage.CreateResponse(reply));
        }

        public void ReplyTo(ITextCommand originalCommand, IAttachment attachment)
        {
            ReplyTo(originalCommand, string.Empty, attachment);
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, IAttachment attachment)
        {
            var msg = originalCommand.InnerMessage.CreateResponse(reply);
            msg.Attachments.Add(attachment);
            ReplyTo(originalCommand, msg);
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, List<IAttachment> attachments)
        {
            var msg = originalCommand.InnerMessage.CreateResponse(reply);
            msg.Attachments.AddRange(attachments);
            ReplyTo(originalCommand, msg);
        }

        public void ReplyTo(ITextCommand originalCommand, IStandardMessage reply)
        {
#pragma warning disable 4014
            DispatchMessage(reply);
#pragma warning restore 4014
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public async Task DispatchMessage(IStandardMessage message)
        {
            Channel targetChannel = null;

            // Is it a PM or a public message?
            if (message.IsPrivate)
            {
                targetChannel = RootClient.PrivateChannels.FirstOrDefault(x => x.Name == message.Channel);
            }
            else
            {
                targetChannel = (RootClient.Servers
                    .FirstOrDefault(x => x.Name == message.Server)?.TextChannels)?.FirstOrDefault(
                        x => x.Name == message.Channel);
            }

            var sentAttachment = false;

            foreach (var attachment in message.Attachments)
            {
                var sendFile = targetChannel?.SendFile(attachment.Name, attachment.DataStream);
                if (sendFile != null)
                    await sendFile;
                sentAttachment = true;
            }

            if (string.IsNullOrEmpty(message.Message) && sentAttachment)
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

        #region Properties

        public bool IsConnected
        {
            get { return RootClient.State == ConnectionState.Connected; }
        }

        public string Name => "Discord.NET Client";

        public string Description => "Client that connects to Discord using the Discord.NET library.";

        public Version Version => new Version(0, 0, 1);
        public bool MarkdownSupported => true;
        public bool AudioSupported => true;
        public bool IrcFormattingSupported => false;
        public bool InlineOrOembedSupported => true;

        public Discord.DiscordClient RootClient { get; }

        #endregion Properties
    }
}