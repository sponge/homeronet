using Homero.Core.EventArgs;
using Homero.Core.Services;
using Homero.Core.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrekQuotes =
    System.Collections.Generic.Dictionary
        <string, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>>;

namespace Homero.Plugin.Goon
{
    public class Trek : IPlugin
    {
        private Random _random = new Random();
        private TrekQuotes _sgQuotes;
        private TrekQuotes _trekQuotes;

        public Trek(IMessageBroker broker)
        {
            _trekQuotes =
                JsonConvert.DeserializeObject<TrekQuotes>(
                    File.ReadAllText(Path.Combine(Paths.ResourceDirectory, "trek.json")));
            _sgQuotes =
                JsonConvert.DeserializeObject<TrekQuotes>(
                    File.ReadAllText(Path.Combine(Paths.ResourceDirectory, "atlantis.json")));
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "trek", "sg" };

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            TrekQuotes quotes = null;
            if (e.Command.Command == "trek")
            {
                quotes = _trekQuotes;
            }
            else if (e.Command.Command == "sg")
            {
                quotes = _sgQuotes;
            }

            var response = "nope";
            string character, count;

            // if character is specified and it's valid, use it, otherwise random
            if (e.Command.Arguments.Count >= 1 && quotes.ContainsKey(e.Command.Arguments[0].ToUpper()))
            {
                character = e.Command.Arguments[0].ToUpper();
            }
            else
            {
                character = quotes.Keys.ElementAt(_random.Next(quotes.Keys.Count));
            }

            // quote for the now valid character
            var charQuotes = quotes[character];

            // if count is specified and it's valid use it, otherwise random
            // TODO: old homero would find the closest length going down, and then going up
            if (e.Command.Arguments.Count >= 2 && charQuotes.ContainsKey(e.Command.Arguments[1]))
            {
                count = e.Command.Arguments[1];
            }
            // else if (argv.count() > 2) -- count is valid, start going down and then up
            else
            {
                count = charQuotes.Keys.ElementAt(_random.Next(charQuotes.Keys.Count));
            }

            // we now have a valid character and word count, select the quote
            var possibleQuotes = charQuotes[count];
            response = possibleQuotes[_random.Next(possibleQuotes.Count)];

            e.ReplyTarget.Send(response);
        }
    }
}