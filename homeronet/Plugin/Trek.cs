using homeronet.Client;
using homeronet.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using TrekQuotes = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>>;

namespace homeronet.Plugin {

    public class Trek : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "trek" };
        private TrekQuotes _quotes;
        private Random _random = new Random();

        public void Startup() {
            using (StreamReader r = new StreamReader("Resources/trek.json")) {
                // TODO: Strict contract
                string json = r.ReadToEnd();
                _quotes = JsonConvert.DeserializeObject<TrekQuotes>(json);
            };
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                var response = "nope";
                string character, count;

                // TODO: i didn't realize we already have command arguments whoops
                var argv = command.InnerMessage.Message.Split();

                // if character is specified and it's valid, use it, otherwise random
                if (argv.Count() > 1 && _quotes.ContainsKey(argv[1].ToUpper())) {
                    character = argv[1].ToUpper();
                }
                else {
                    character = _quotes.Keys.ElementAt(_random.Next(_quotes.Keys.Count));
                }

                // quote for the now valid character
                var charQuotes = _quotes[character];

                // if count is specified and it's valid use it, otherwise random
                // TODO: old homero would find the closest length going down, and then going up
                if (argv.Count() > 2 && charQuotes.ContainsKey(argv[2].ToString())) {
                    count = argv[2].ToString();
                }
                // else if (argv.count() > 2) -- count is valid, start going down and then up
                else {
                    count = charQuotes.Keys.ElementAt(_random.Next(charQuotes.Keys.Count));
                }

                // we now have a valid character and word count, select the quote
                var quotes = charQuotes[count];
                response = quotes[_random.Next(quotes.Count)];

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