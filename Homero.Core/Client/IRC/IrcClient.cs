using Homero.Core.EventArgs;
using Homero.Core.Interface;
using Homero.Core.Services;
using IrcDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homero.Core.Client.IRC
{
    public class IrcClient : IClient
    {
        private List<StandardIrcClient> _clients;
        private IConfiguration _config;
        private ILogger _logger;
        private IUploader _uploader;
        public IrcClient(IConfiguration config, ILogger logger, IUploader uploader)
        {
            _config = config;
            _logger = logger;
            _uploader = uploader;
            _clients = new List<StandardIrcClient>();
        }

        public event EventHandler<MessageEventArgs> MessageReceived;

        public event EventHandler<MessageEventArgs> MessageSent;

        public bool AudioSupported => false;

        public string Description => "IRC Client using IrcDotNet";

        public bool InlineOrOembedSupported => false;

        public bool IrcFormattingSupported => true;

        public bool IsConnected
        {
            get { return _clients.Count > 0; }
        }

        public bool MarkdownSupported => false;

        public string Name => "IRC Client";

        public List<IServer> Servers
        {
            get
            {
                return _clients.Select(x => new IrcServer(x, _uploader)).Cast<IServer>().ToList();
            }
        }

        public Version Version => new Version(0, 0, 1);

        public Task<bool> Connect()
        {
            foreach (IrcServerConfiguration serverConfig in _config.GetValue<List<IrcServerConfiguration>>("servers"))
            {
                StandardIrcClient client = new StandardIrcClient();
                client.Connected += ClientOnConnected;
                client.ConnectFailed += ClientOnConnectFailed;
                client.Connect(new Uri($"irc://{serverConfig.Host}"), new IrcUserRegistrationInfo()
                {
                    NickName = serverConfig.Nickname,
                    UserName = serverConfig.Username,
                    RealName = serverConfig.RealName
                });
            }
            return new Task<bool>(() => true); // Future todo: actually return a real result, right now we don't care because we're jerks
        }

        private void ClientOnConnected(object sender, System.EventArgs eventArgs)
        {
            StandardIrcClient client = sender as StandardIrcClient;
            if (client == null)
            {
                return;
            }

            _clients.Add(client);
            client.LocalUser.JoinedChannel += LocalUserOnJoinedChannel;

            // Join all known channels. This sucks.
            foreach (
                IrcServerConfiguration serverConfig in
                    _config.GetValue<List<IrcServerConfiguration>>("servers")
                        .Where(x => x.Host.Contains(client.ServerName)))
            {
                foreach (string channel in serverConfig.Channels)
                {
                    client.Channels.Join(channel);
                }
            }
        }

        private void LocalUserOnJoinedChannel(object sender, IrcChannelEventArgs e)
        {
            e.Channel.MessageReceived += (s, args) =>
            {
                ChannelOnMessageReceived(s, args, e.Channel);
            };
        }

        private void ChannelOnMessageReceived(object sender, IrcMessageEventArgs e, IrcDotNet.IrcChannel channel)
        {
            StandardIrcClient client = sender as StandardIrcClient;

            if (client == null)
            {
                return;
            }

            MessageEventArgs args = new MessageEventArgs(new IrcMessage(e.Text), null,
                new IrcChannel(channel, _uploader),
                new IrcUser(channel.Users.FirstOrDefault(x => x.User.NickName == e.Source.Name)?.User, _uploader));

            // Since we can't really listen to outgoing accurately, we can abuse this to get our message sent event. 
            if (e.Source.Name == client.LocalUser.NickName)
            {
                MessageSent?.Invoke(this, args);
                return;
            }

            MessageReceived?.Invoke(this, args);
        }

        private void ClientOnConnectFailed(object sender, IrcErrorEventArgs ircErrorEventArgs)
        {
            _logger.Error(ircErrorEventArgs.Error.Message);
        }
    }
}