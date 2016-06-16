﻿using homeronet.EventArgs;
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


        #endregion Constructors

        #region Events

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion Events

        #region Async Methods

        public async Task<bool> Connect()
        {
            await _discordClient.Connect(ClientConfiguration.ApiKey);
            return true; // uh why can't i get the connect result?
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public async Task SendMessage(IStandardMessage message)
        {
            // Is it a PM or a public message?
            if (message.IsPrivate)
            {
                var targetChannel = _discordClient.PrivateChannels.FirstOrDefault(x => x.Name == message.Channel);
                var sendMessage = targetChannel?.SendMessage(message.Message);
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

        private void DiscordClientOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            DiscordMessage message = new DiscordMessage(this, messageEventArgs.Message);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
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

        public string Name => "Discord.NET Client";

        public string Description => "Client that connects to Discord using the Discord.NET library.";

        public Version Version => new Version(0,0,1);

        public Discord.DiscordClient RootClient
        {
            get { return _discordClient; }
        }

        #endregion Properties
    }
}