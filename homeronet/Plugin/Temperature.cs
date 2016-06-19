using homeronet.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace homeronet.Plugin {

    public class Temperature : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "temperature" };

        public void Startup() {
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                double temp;
                try {
                    temp = double.Parse(command.Arguments[0]);
                } catch (Exception) {
                    return command.InnerMessage.CreateResponse(".temperature <temp> -- converts <temp> from C to F and F to C");
                }

                if (Math.Abs(temp) == 420) {
                    return command.InnerMessage.CreateResponse("SMOKE WEED EVERY DAY DONT GIVE A FUCK");
                } else if(Math.Abs(temp) > 500) {
                    return command.InnerMessage.CreateResponse("2 hot 4 u");
                }

                double c = (temp - 32) * (5.0 / 9.0);
                double f = (temp * (9.0 / 5.0)) + 32;

                return command.InnerMessage.CreateResponse(String.Format("{0:0.0}F is {1:0.0}C. {0:0.0}C is {2:0.0}F.", temp, c, f));
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