﻿using System;
using System.Collections.Generic;
using Homero.Client;
using Homero.EventArgs;
using Homero.Services;

namespace Homero.Plugin.Converter {
    public class Saturn : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "saturn" };

        private Dictionary<char, char> _saturnReplacements = new Dictionary<char, char>() {
            { 'a', 'ᗩ'},
            { 'b', 'ᗷ'},
            { 'c', 'ᘓ'},
            { 'd', 'ᗪ'},
            { 'e', 'ᕮ'},
            { 'f', 'ᖴ'},
            { 'g', 'ᕤ'},
            { 'h', 'ᗁ'},
            { 'i', 'ᓮ'},
            { 'j', 'ᒎ'},
            { 'k', 'ᔌ'},
            { 'l', 'ᒪ'},
            { 'm', 'ᙏ'},
            { 'n', 'ᑎ'},
            { 'o', 'ᘎ'},
            { 'p', 'ᖘ'},
            { 'q', 'ᕴ'},
            { 'r', 'ᖇ'},
            { 's', 'ᔕ'},
            { 't', 'ᒮ'},
            { 'u', 'ᘮ'},
            { 'v', 'ᐯ'},
            { 'w', 'ᙎ'},
            { 'x', '᙭'},
            { 'y', 'ᖿ'},
            { 'z', 'ᔓ'},
            { '\'', 'ᐞ'}
        };

        public Saturn(IMessageBroker broker) {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            if (e.Command.Arguments?.Count == 0) {
                return;
            }

            var str = String.Join(" ", e.Command.Arguments).ToLower();

            foreach (KeyValuePair<char, char> entry in _saturnReplacements) {
                str = str.Replace(entry.Key, entry.Value);
            }

            client?.ReplyTo(e.Command, str);
        }

    }
}