﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;
using System.Text.RegularExpressions;

namespace Homero.Plugin.Media {
    public class YouTubeDubber : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "lawnmower", "whale", "cow", "worldstar", "dub" };
        private Regex _ytRegex;

        private Dictionary<string, string> _builtins = new Dictionary<string, string>() {
            {"whale", "http://www.youtube.com/watch?v=ZS_6-IwMPjM"},
            {"cow", "http://www.youtube.com/watch?v=lXKDu6cdXLI"},
            {"lawnmower", "http://www.youtube.com/watch?v=r6FpEjY1fg8"},
            {"worldstar", "https://www.youtube.com/watch?v=uEgtNSBa4Zk"}
        };

        public YouTubeDubber(IMessageBroker broker) {
            broker.CommandReceived += Broker_CommandReceived;
            _ytRegex = new Regex("(?:http|https)://(?:www\\.)?(?:youtube\\.com/watch\\?v=|youtu\\.be/)([^&\n]+)", RegexOptions.Compiled);
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        public string Dub(string video, string audio, int delay) {
            var videoMatch = _ytRegex.Match(video);
            var audioMatch = _ytRegex.Match(audio);

            if (videoMatch.Groups.Count != 2 && audioMatch.Groups.Count != 2) {
                return "couldn't parse that one m8";
            }

            video = videoMatch.Groups[1].Value;
            audio = audioMatch.Groups[1].Value;

            return $"http://www.youdubber.com/index.php?video={video}&audio={audio}&audio_start={delay}";
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            var delay = 0;

            var minArgCount = e.Command.Command == "dub" ? 2 : 1;

            if (e.Command.Arguments.Count < minArgCount) {
                client?.ReplyTo(e.Command, ".dub < vid > < audio > [audio start time]-- tubedubber");
                return;
            }

            if (e.Command.Arguments.Count > minArgCount) {
                var success = int.TryParse(e.Command.Arguments[minArgCount], out delay);
                if (!success) {
                    client?.ReplyTo(e.Command, "that is not a time");
                    return;
                }
            }

            string video = null, audio = null;
            if (e.Command.Command == "dub") {
                video = e.Command.Arguments[0];
                audio = e.Command.Arguments[1];
            }
            else if (e.Command.Command == "worldstar") {
                video = e.Command.Arguments[0];
                audio = _builtins[e.Command.Command];
            }
            else {
                video = _builtins[e.Command.Command];
                audio = e.Command.Arguments[0];
            }

            var strOut = Dub(video, audio, delay);

            client?.ReplyTo(e.Command, strOut);
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}