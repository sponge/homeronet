using DiscordSharp;
using DiscordSharp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace homeronet.plugins {

    internal class tone : IPlugin {

        public string GetCommandTrigger() {
            return "tone";
        }

        public void CommandTrigger(DiscordMessageEventArgs e) {
            e.Channel.SendMessage("Tony is sitting opposite you, examinig each of his fingers in turn.");
            Thread.Sleep(1000);
            e.Channel.SendMessage("You wish you could put Tony out of his misery.");
            Thread.Sleep(1000);
            e.Channel.SendMessage("You isl you could put Tony out of h5s mgsery.");
            Thread.Sleep(1000);
            e.Channel.SendMessage("You wis5 yougco6ld put T4ny out ofchis 4iser7.");
            Thread.Sleep(1000);
            e.Channel.SendMessage("Yo4 wis5hyobgcokldsp4t T46y 3ut ofc7is 4is5r74");
            Thread.Sleep(1000);
            e.Channel.SendMessage("Y44 wis5hy3bg5o5ld7p44 T464444444fc7is44i454744");
            Thread.Sleep(1000);
            e.Channel.SendMessage("54783il5hy3bg5o55d788888864444444f37is24i454744");
            Thread.Sleep(1000);
            e.Channel.SendMessage("----------- rest in peace tony -----------");
        }

        public void Shutdown() {
            Console.WriteLine("shutting down tone plugin");
        }

        public void Startup(DiscordClient client) {
            Console.WriteLine("starting tone plugin");
        }
    }
}