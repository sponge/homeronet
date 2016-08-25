using Homero.Core.EventArgs;
using Homero.Core.Services;
using Homero.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Homero.Plugin.Goon
{
    public class Fortune : IPlugin
    {
        private List<FortuneFile> _fortuneFiles;
        private Dictionary<string, List<string>> _fortunes = new Dictionary<string, List<string>>();
        private Random _random = new Random();

        public Fortune(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
            // TODO: should we strip messages > 500 chars? was that an irc thing or a not annoying the channel thing?
            _fortuneFiles = new List<FortuneFile>
            {
                new FortuneFile("business", Path.Combine(Paths.ResourceDirectory, "business.txt"), false, false),
                new FortuneFile("deepthought", Path.Combine(Paths.ResourceDirectory, "deepthoughts.txt"), false, true),
                new FortuneFile("fatgoon", Path.Combine(Paths.ResourceDirectory, "fatgoon.txt"), true, true),
                new FortuneFile("jerk", Path.Combine(Paths.ResourceDirectory, "jerk.txt"), false, false),
                new FortuneFile("truth", Path.Combine(Paths.ResourceDirectory, "trolldb.txt"), true, true)
            };

            var pattern = new Regex("\r\n?|\n");

            foreach (var fortuneFile in _fortuneFiles)
            {
                var text = File.ReadAllText(fortuneFile.Path);
                RegisteredTextCommands.Add(fortuneFile.Command);

                var stringList =
                    text.Split(new[] { fortuneFile.IsMultiLine ? "\n%" : "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(str => str.Trim());

                if (fortuneFile.StripNewLines)
                {
                    stringList = stringList.Select(str => pattern.Replace(str, " "));
                }

                _fortunes[fortuneFile.Command] = stringList.ToList();
            }
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string>();

        public void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            if (_fortunes.ContainsKey(e.Command.Command))
            {
                e.ReplyTarget.Send(_fortunes[e.Command.Command][_random.Next(_fortunes.Count)]);
            }
        }
    }
}