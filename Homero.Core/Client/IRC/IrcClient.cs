using Homero.Core.EventArgs;
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
        private Dictionary<StandardIrcClient, IrcServerConfiguration> _clients;
        private IConfiguration _config;
        private ILogger _logger;
        private IUploader _uploader;

        public IrcClient(IConfiguration config, ILogger logger, IUploader uploader)
        {
            _config = config;
            _logger = logger;
            _uploader = uploader;
            _clients = new Dictionary<StandardIrcClient, IrcServerConfiguration>();
        }

        public event EventHandler<MessageEventArgs> MessageReceived;

        public event EventHandler<MessageEventArgs> MessageSent;

        public string Description => "IRC Client using IrcDotNet";

        public ClientFeature Features => ClientFeature.Text | ClientFeature.ColorControlCodes;

        public bool IsConnected
        {
            get { return _clients.Count > 0; }
        }

        public string Name => "IRC Client";

        public List<IServer> Servers
        {
            get
            {
                return _clients.Keys.Select(x => new IrcServer(x, _uploader)).Cast<IServer>().ToList();
            }
        }

        public Version Version => new Version(0, 0, 1);

        public Task<bool> Connect()
        {
            foreach (IrcServerConfiguration serverConfig in _config.GetValue<List<IrcServerConfiguration>>("servers"))
            {
                StandardIrcClient client = new StandardIrcClient();
                _clients[client] = serverConfig;
                client.Connected += ClientOnConnected;
                client.ConnectFailed += ClientOnConnectFailed;
                client.Registered += ClientOnRegistered;

                client.Connect(new Uri($"irc://{serverConfig.Host}"), new IrcUserRegistrationInfo()
                {
                    NickName = serverConfig.Nickname,
                    UserName = serverConfig.Username,
                    RealName = serverConfig.RealName,
                });
            }
            return new Task<bool>(() => true); // Future todo: actually return a real result, right now we don't care because we're jerks
        }

        private void ClientOnRegistered(object sender, System.EventArgs eventArgs)
        {
            StandardIrcClient client = sender as StandardIrcClient;
            if (client == null)
            {
                return;
            }

            client.LocalUser.JoinedChannel += LocalUserOnJoinedChannel;
            IrcServerConfiguration serverConfiguration = _clients[client];

            if (serverConfiguration != null)
            {
                if (!string.IsNullOrEmpty(serverConfiguration.NickServPassword))
                {
                    client.LocalUser.SendMessage(serverConfiguration.NickservUsername, serverConfiguration.NickServPassword);
                }

                foreach (string channel in serverConfiguration.Channels)
                {
                    client.Channels.Join(channel);
                }
            }
        }

        private void ClientOnConnected(object sender, System.EventArgs eventArgs)
        {
            StandardIrcClient client = sender as StandardIrcClient;
            if (client == null)
            {
                return;
            }

            client.GetNetworkInfo();
            client.GetServerLinks();
        }

        private void LocalUserOnJoinedChannel(object sender, IrcChannelEventArgs e)
        {
            e.Channel.MessageReceived += (s, args) =>
            {
                ChannelOnMessageReceived(sender, args, e.Channel);
            };
        }

        private void ChannelOnMessageReceived(object sender, IrcMessageEventArgs e, IrcDotNet.IrcChannel channel)
        {
            IrcLocalUser localUser = sender as IrcLocalUser;

            if (localUser == null)
            {
                return;
            }

            MessageEventArgs args = new MessageEventArgs(new IrcMessage(e.Text), new IrcServer(_clients.Keys.First(x => x.LocalUser.Equals(localUser)), _uploader),
                new IrcChannel(channel, _uploader),
                new IrcUser(channel.Users.FirstOrDefault(x => x.User.NickName == e.Source.Name)?.User, _uploader));

            // Since we can't really listen to outgoing accurately, we can abuse this to get our message sent event.
            if (e.Source.Name == localUser.NickName)
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