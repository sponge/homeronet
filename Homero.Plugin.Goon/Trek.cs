using Homero.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Services;
using Homero.Utility;
using Newtonsoft.Json;
using TrekQuotes = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>>;

namespace Homero.Plugin.Goon
{
    public class Trek : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() { "trek" };
        private TrekQuotes _quotes;
        private Random _random = new Random();

        public Trek(IMessageBroker broker)
        {
            _quotes = JsonConvert.DeserializeObject<TrekQuotes>(File.ReadAllText(Path.Combine(Paths.ResourceDirectory,"trek.json")));
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;

            var response = "nope";
            string character, count;

            // if character is specified and it's valid, use it, otherwise random
            if (e.Command.Arguments.Count >= 1 && _quotes.ContainsKey(e.Command.Arguments[0].ToUpper()))
            {
                character = e.Command.Arguments[0].ToUpper();
            }
            else
            {
                character = _quotes.Keys.ElementAt(_random.Next(_quotes.Keys.Count));
            }

            // quote for the now valid character
            var charQuotes = _quotes[character];

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
            var quotes = charQuotes[count];
            response = quotes[_random.Next(quotes.Count)];

            client?.ReplyTo(e.Command, response);
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

    }
}