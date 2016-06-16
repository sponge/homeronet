using homeronet.EventArgs;
using homeronet.Messages;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.API.Client;
using User = Discord.User;

namespace homeronet.Client
{
    public class DiscordClient : IClient
    {

        private Discord.DiscordClient _discordClient;
        private IClientConfiguration _clientConfiguration;

        #region Constructors

        public DiscordClient(IClientConfiguration config)
        {
            ClientConfiguration = config;

            if (String.IsNullOrEmpty(ClientConfiguration.ApiKey))
            {
                throw new Exception("No API key specified.");
            }


            _discordClient = new Discord.DiscordClient(x =>
            {
                // Seriously, a configuration constructor in an action?
                x.AppName = "Homero.NET";
                x.AppUrl = "https://goodass.dog";
            });

            _discordClient.MessageReceived += DiscordClientOnMessageReceived;

        }

        private void DiscordClientOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
#if DEBUG
            Debug.WriteLine(messageEventArgs.Message);
#endif
            DiscordMessage message = new DiscordMessage(messageEventArgs.Message);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion Events

        #region Async Methods

        public async Task<bool> Connect()
        {
            await _discordClient.Connect(ClientConfiguration.ApiKey);
            return true; // uh why can't i get the connect result?
        }

        public async Task SendMessage(IStandardMessage message)
        {
            // Is it a PM or a public message?
            if (message.IsPrivate)
            {
                User targetedUser = null;
                foreach (Server server in _discordClient.Servers)
                {
                    targetedUser = server.Users.FirstOrDefault(x => x.Nickname == message.Sender);
                }
                var sendMessage = targetedUser?.SendMessage(message.Message);
                if (sendMessage != null)
                    await sendMessage;
            }
            else
            {
                Discord.Channel targetedChannel = (_discordClient.Servers
                    .FirstOrDefault(x => x.Name == message.Server)?.TextChannels)?.FirstOrDefault(x => x.Name == message.Channel);

                var sendMessage = targetedChannel?.SendMessage(message.Message);
                if (sendMessage != null)
                    await sendMessage;
            }
        }

        #endregion Async Methods

        #region Methods

        public void Initialize()
        {
            // Not used....yet.
        }

        #endregion Methods

        #region Properties

        public bool IsConnected
        {
            get
            {
                return _discordClient.State == ConnectionState.Connected;
            }
        }

        public IClientConfiguration ClientConfiguration
        {
            get { return _clientConfiguration; }
            set { _clientConfiguration = value; }
        }

        #endregion Properties
    }
}