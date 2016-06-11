using DiscordSharp;
using DiscordSharp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.plugins {

    internal interface IPlugin {

        string GetCommandTrigger();

        void CommandTrigger(DiscordMessageEventArgs e);

        void Startup(DiscordClient client);

        void Shutdown();
    }
}