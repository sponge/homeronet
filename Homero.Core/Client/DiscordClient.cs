using Discord;
using Homero.Core.Interface;
using Homero.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            RootClient.MessageSent += DiscordClientOnMessageSent;
        }

        private void DiscordClientOnMessageSent(object sender, Discord.MessageEventArgs e)
        {
            MessageSent?.Invoke(this, new EventArgs.MessageEventArgs(new DiscordMessage(e.Message), new DiscordServer(e.Server), new DiscordChannel(e.Channel), new DiscordUser(e.User)));
        }

        #endregion Constructors

        #region Methods

        private void DiscordClientOnMessageReceived(object sender, Discord.MessageEventArgs e)
        {
            if (e.User.Id == RootClient.CurrentUser.Id)
            {
                return;
            }
            MessageReceived?.Invoke(this, new EventArgs.MessageEventArgs(new DiscordMessage(e.Message), new DiscordServer(e.Server), new DiscordChannel(e.Channel), new DiscordUser(e.User)));
        }

        #endregion Methods

        #region Events

        public event EventHandler<EventArgs.MessageEventArgs> MessageReceived;

        public event EventHandler<EventArgs.MessageEventArgs> MessageSent;

        #endregion Events

        public List<IServer> Servers
        {
            get { return RootClient.Servers.Select(x => new DiscordServer(x)).Cast<IServer>().ToList(); }
        }

        public async Task<bool> Connect()
        {
            await RootClient.Connect(_config.GetValue<string>("key"));
            return true; // uh why can't i get the connect result?
        }

        #region Properties

        public bool AudioSupported => true;

        public string Description => "Client that connects to Discord using the Discord.NET library.";

        public bool InlineOrOembedSupported => true;

        public bool IrcFormattingSupported => false;

        public bool IsConnected
        {
            get { return RootClient.State == ConnectionState.Connected; }
        }

        public bool MarkdownSupported => true;

        public string Name => "Discord.NET Client";

        public Discord.DiscordClient RootClient { get; }

        public Version Version => new Version(0, 0, 1);

        #endregion Properties
    }
}