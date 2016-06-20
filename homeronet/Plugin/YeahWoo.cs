using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Messages;

namespace Homero.Plugin {

    public class YeahWoo : IPlugin {
        private List<string> _registeredCommands = new List<string>() { };

        public void Startup() {
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return null;
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return new Task<IStandardMessage>(() => {
                if (message.Message == "yeah") {
                    return message.CreateResponse("woo");
                }
                else if (message.Message == "woo") {
                    return message.CreateResponse("yeah");
                }
                    return null;
            });
        }
    }
}