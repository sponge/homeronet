using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Homero.Core.Interface;

namespace Homero.Plugin.Converter
{
    public class CodeEval : IPlugin
    {
        private WebClient _ws;

        public CodeEval(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
            _ws = new WebClient();
            _ws.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "python" };

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            if (e.Command.Arguments.Count == 0)
            {
                e.ReplyTarget.Send($"need some code to run");
                return;
            }

            var options = new Dictionary<string, string>
            {
                {"code", string.Join(" ", e.Command.Arguments)},
                {"options", ""},
                {"compiler", "python-2.7.3"},
                {"compiler-option-raw", ""}
            };

            var optStr = JsonConvert.SerializeObject(options);

            var resultStr = _ws.UploadString("http://melpon.org/wandbox/api/compile.json", optStr);

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultStr);

            if (int.Parse(result["status"]) != 0)
            {
                e.ReplyTarget.Send(result["program_error"]);
            }
            else
            {
                e.ReplyTarget.Send(result["program_output"]);
            }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}