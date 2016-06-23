using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Messages.Attachments;
using Homero.Services;
using IrcDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Homero.Client
{
    public class IrcClient : IClient
    {
        private StandardIrcClient _ircClient;
        private IConfiguration _config;
        private ILogger _logger;
        private bool _isConnected = false;

        public IrcClient(IConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _ircClient = new StandardIrcClient();

            _ircClient.Registered += IrcClientOnConnectionComplete;
            _ircClient.RawMessageSent += (s, e) => _logger.Info(e.RawContent);
            _ircClient.RawMessageReceived += (s, e) => _logger.Info(e.RawContent);
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageSentEventArgs> MessageSent;

        private void IrcClientOnConnectionComplete(object sender, System.EventArgs eventArgs)
        {
            _ircClient.LocalUser.JoinedChannel += (s, e) =>
            {
                e.Channel.MessageReceived += (ss, ee) => IrcClientOnMessageReceived(ss, ee, e.Channel);
            };
            _ircClient.Channels.Join(_config.GetValue<string>("channel"));
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
        public string Description => "what up dogggg";
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

            _ircClient.LocalUser.SendMessage(channel, message.Message);
            MessageSent?.Invoke(this, new MessageSentEventArgs(message));

            foreach (IAttachment attachment in message.Attachments)
            {
                if (attachment is ImageAttachment)
                {
                    string image = await UploadImageToImgur(attachment);
                    _ircClient.LocalUser.SendMessage(channel, image);
                    //  MessageSent?.Invoke(this, new MessageSentEventArgs(message));
                }
            }
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
            throw new NotImplementedException();
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, List<IAttachment> attachments)
        {
            throw new NotImplementedException();
        }

        public void ReplyTo(ITextCommand originalCommand, string reply, IAttachment attachment)
        {
            IStandardMessage message = originalCommand.InnerMessage.CreateResponse(reply);
            message.Attachments.Add(attachment);
            ReplyTo(originalCommand, message);
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, List<IAttachment> attachments)
        {
            throw new NotImplementedException();
        }

        public void ReplyTo(IStandardMessage originalMessage, string reply, IAttachment attachment)
        {
            throw new NotImplementedException();
        }

        private async Task<string> UploadImageToImgur(IAttachment attachment)
        {
            using (HttpClient client = new HttpClient())
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(attachment.Data), "image");

                client.DefaultRequestHeaders.Add("Authorization", "Client-ID " + "2aae75cdaf31d78");

                HttpResponseMessage response = await client.PostAsync("https://api.imgur.com/3/image", content);
                string jsonSource = await response.Content.ReadAsStringAsync();
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(jsonSource);

                if ((bool)jsonObj.GetValue("success"))
                {
                    return jsonObj.SelectToken("data.link").ToString();
                }
                return null;
            }
        } 
    }
}
