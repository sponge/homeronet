using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Messages;

namespace Homero.Plugin {

    public class Beats : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "beats" };

        public void Startup() {
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                var now = DateTime.UtcNow;
                var utcTime = now.ToString("H:mm:ss");
                var beatsTime = Math.Floor((now.Second + (now.Minute * 60) + (now.Hour * 3600)) / 86.4f);
                return command.InnerMessage.CreateResponse($"utc time: {utcTime} | beat time: @{beatsTime}");
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