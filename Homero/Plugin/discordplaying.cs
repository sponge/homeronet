using DiscordSharp;
using DiscordSharp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace homeronet.plugins {

    internal class discordplaying : IPlugin {
        private DiscordClient client;

        public string GetCommandTrigger() {
            return "playing";
        }

        public void CommandTrigger(DiscordMessageEventArgs e) {
            var parts = e.MessageText.ToString().Split(null, 2);
            if (parts.Length == 2) {
                client.UpdateCurrentGame(parts[1]);
                e.Channel.SendMessage("thanks, on discord i'm now playing " + parts[1]);
            }
            else {
                e.Channel.SendMessage("playing: set my discord now playing status");
            }
        }

        public void Shutdown() {
            Console.WriteLine("shutting down discord playing plugin");
        }

        public void Startup(DiscordClient client) {
            this.client = client;
            Console.WriteLine("starting discord playing plugin");
        }
    }
}