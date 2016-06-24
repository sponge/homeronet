using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;
using Newtonsoft.Json;
using System.IO;
using Homero.Utility;
using HysDB = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>;

namespace Homero.Plugin.Goon {
    public class Hys : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "hys" };
        private Random _random;

        public Hys(IMessageBroker broker) {
            broker.CommandReceived += Broker_CommandReceived;
            _random = new Random();
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        private string Choice(HysDB db, string key) {
            return db[key][_random.Next(db[key].Count)];
        }

        private string GetPunctuation() {
            var r = _random.Next(5);

            switch (r) {
                case 0:
                    return ",, ";
                case 1:
                case 2:
                    return ". ";
                case 3:
                case 4:
                    return ", ";
                default:
                    return "... ";
            };         
        }

        public string CreateIdiot(string name) {
            var hys = JsonConvert.DeserializeObject<HysDB>(File.ReadAllText(Path.Combine(Paths.ResourceDirectory, "hys.json")));

            var idiotText = "";
            if (name.Length == 0) {
                idiotText += $"{Choice(hys, "opening2")} {Choice(hys, "hated_object")} {Choice(hys, "terrible_thing")}";
            } else {
                var plural = name.EndsWith("s") ? "are" : "is";
                idiotText += $"{Choice(hys, "opening2")} {name} {plural} {Choice(hys, "terrible_thing")}";
            }

            if (_random.NextDouble() > 0.5) {
                idiotText += $" because {Choice(hys, "because")}";
            }

            idiotText += $"{GetPunctuation()} {Choice(hys, "moronic_solution")} {GetPunctuation()}";

            if (_random.NextDouble() > 0.4) {
                idiotText += $"{Choice(hys, "signoff")}{Choice(hys, "end_punctuation")}";
            }
            else {
                idiotText += $"{Choice(hys, "opening1")}{GetPunctuation()}{idiotText}";
            }

            if (_random.NextDouble() > 0.3f) {
                idiotText = idiotText.ToUpper();
            }

            return idiotText;
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            var outStr = CreateIdiot(String.Join(" ", e.Command.Arguments));
            client?.ReplyTo(e.Command, outStr);
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}