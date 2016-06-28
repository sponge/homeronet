using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Homero.Core.EventArgs;
using Homero.Core.Interface;
using Homero.Core.Messages;
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
            RootClient.MessageSent += RootClient_MessageSent;
        }

        private void RootClient_MessageSent(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
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

        public List<IServer> Servers
        {
            get { return RootClient.Servers.Select(x => new DiscordServer(x)).Cast<IServer>().ToList(); }
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