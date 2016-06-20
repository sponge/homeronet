using homeronet.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace homeronet.Plugin {

    public class SBEmail : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "sbemail" };

        public void Startup() {
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                var rnd = new Random().Next(206);
                return command.InnerMessage.CreateResponse($"http://www.homestarrunner.com/sbemail{rnd}.html");
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