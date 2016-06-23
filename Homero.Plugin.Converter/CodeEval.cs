using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.Messages;
using Homero.Services;
using System.Net;
using Newtonsoft.Json;

namespace Homero.Plugin.Converter {
    public class CodeEval : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "python" };
        WebClient _ws;

        public CodeEval(IMessageBroker broker) {
            broker.CommandReceived += Broker_CommandReceived;
            _ws = new WebClient();
            _ws.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        private void Broker_CommandReceived(object sender, EventArgs.CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            if (e.Command.Arguments.Count == 0) {
                client?.ReplyTo(e.Command, $"need some code to run");
                return;
            }

            Dictionary<string, string> options = new Dictionary<string, string>() {
                {"code", string.Join(" ", e.Command.Arguments) },
                {"options", ""},
                {"compiler", "python-2.7.3"},
                {"compiler-option-raw", ""}
            };

            var optStr = JsonConvert.SerializeObject(options);
            
            var resultStr = _ws.UploadString("http://melpon.org/wandbox/api/compile.json", optStr);

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultStr);

            if (int.Parse(result["status"]) != 0) {
                client?.ReplyTo(e.Command, result["program_error"]);
            } else {
                client?.ReplyTo(e.Command, result["program_output"]);
            }
            
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}