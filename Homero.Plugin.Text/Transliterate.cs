using Homero.Core.Services;
using Homero.Core.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.EventArgs;
using System.Text.RegularExpressions;

namespace Homero.Plugin.Text
{
    public class Transliterate : IPlugin
    {
        private class TransliterateMapping
        {
            public List<List<string>> Multi { get; set; }
            public Dictionary<string, string> Single { get; set; }
        }

        private Dictionary<string, TransliterateMapping> _mappings;

        public List<string> RegisteredTextCommands
        {
            get
            {
                return _mappings.Keys.ToList();
            }
        }

        public Transliterate(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            e.ReplyTarget.Send(ApplyMappings(_mappings[e.Command.Command], String.Join(" ", e.Command.Arguments).ToLower()));
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public void Startup()
        {
            _mappings = JsonConvert.DeserializeObject<Dictionary<string, TransliterateMapping>>(File.ReadAllText(Path.Combine(Paths.ResourceDirectory, "TransliterateMappings.json")));
        }

        private string ApplyMappings(TransliterateMapping mappings, string input)
        {
            // this is to make a regex like /abc[^\w]/ work on the last word in the input
            input += " ";
            StringBuilder output = new StringBuilder();

            List<string> searches = mappings.Multi.Select(x => x.First()).ToList();

            foreach(string word in input.Split(' '))
            {
                StringBuilder wordBuilder = new StringBuilder(word);
                while (wordBuilder.Length > 0)
                {
                    string match = searches.Where(x => word.Contains(x)).FirstOrDefault();
                    if (match != null)
                    {
                        wordBuilder.Remove(0, match.Length);
                        output.Append(match);
                    }
                    else
                    {
                        output.Append(mappings.Single[wordBuilder[0].ToString()]);
                        wordBuilder.Remove(0, 1);
                    }
                }
                output.Append(' ');
            }

            //foreach(List<string> mapping in mappings.Multi)
            //{
            //    input = Regex.Replace(input, mapping.First(), mapping.Last());
            //}

            //foreach (KeyValuePair<string, string> mapping in mappings.Single)
            //{
            //    input = input.Replace(mapping.Key, mapping.Value);
            //}

            // this doesn't a stringbuilder because i need to regex replace and i can't do that in place in a
            // stringbuilder. don't think the speed matters on this small a set of replacements.
            return output.ToString();
        }
    }
}
