using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Messages;

namespace Homero.Plugin {

    public class Pepito : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "pepito" };

        public void Startup() {
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                var amt = new Random().Next(68, 421);
                string hooray = amt == 100 ? "💯" : amt.ToString();
                return command.InnerMessage.CreateResponse($"<peptio> hey guys i just ate {hooray} pills");
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