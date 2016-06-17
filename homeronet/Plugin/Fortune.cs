﻿using homeronet.Client;
using homeronet.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace homeronet.Plugin {

    public class Fortune : IPlugin {
        private List<string> _registeredCommands = new List<string>();
        private Dictionary<string, List<string>> _fortunes = new Dictionary<string, List<string>>();
        private Random _random = new Random();

        private struct FortuneFile {
            public String command;
            public String path;
            public bool stripNewLines;
            public bool isMultiline;

            public FortuneFile(string command, string path, bool stripNewLines, bool isMultiline) : this() {
                this.command = command;
                this.path = path;
                this.stripNewLines = stripNewLines;
                this.isMultiline = isMultiline;
            }
        }

        private List<FortuneFile> _fortuneFiles = new List<FortuneFile>()
        {
            new FortuneFile("business", "Resources/business.txt", false, false),
            new FortuneFile("deepthought", "Resources/deepthoughts.txt", false, true),
            new FortuneFile("fatgoon", "Resources/fatgoon.txt", true, true),
            new FortuneFile("jerk", "Resources/jerk.txt", false, false),
            new FortuneFile("truth", "Resources/trolldb.txt", true, true),
        };

        public void Startup() {
            // TODO: should we strip messages > 500 chars? was that an irc thing or a not annoying the channel thing?
            var pattern = new Regex("\r\n?|\n");

            foreach (var fortuneFile in _fortuneFiles) {
                var text = File.ReadAllText(fortuneFile.path);
                _registeredCommands.Add(fortuneFile.command);

                var stringList = text.Split(new string[] { fortuneFile.isMultiline ? "\n%" : "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim());

                if (fortuneFile.stripNewLines) {
                    stringList = stringList.Select(str => pattern.Replace(str, " "));
                }

                _fortunes[fortuneFile.command] = stringList.ToList();
            }
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                var response = "nope";
                if (_fortunes.ContainsKey(command.Command)) {
                    response = _fortunes[command.Command][_random.Next(_fortunes.Count)];
                }

                return command.InnerMessage.CreateResponse(response);
            });
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}