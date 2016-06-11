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

    internal class homero : IPlugin {

        public string GetCommandTrigger() {
            return "homero";
        }

        public void CommandTrigger(DiscordMessageEventArgs e) {
            WebRequest req = WebRequest.Create("http://simpsons-latino.tumblr.com/random");
            WebResponse response = req.GetResponse();
            e.Channel.SendMessage(response.ResponseUri.ToString());
        }

        public void Shutdown() {
            Console.WriteLine("shutting down homero plugin");
        }

        public void Startup(DiscordClient client) {
            Console.WriteLine("starting homero plugin");
        }
    }
}