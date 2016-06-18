using homeronet.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace homeronet.Plugin {

    public class Thinker : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "think" };
        private string thinker = "( .   __ . ) . o O ( {0} )\n";
        private string emptyLn = "                     {0}\n";

        public void Startup() {
            Program.Log.Info("( .__. ).o O( thinkin real hard about startin' up )");
        }

        public void Shutdown() {
            Program.Log.Info("( .__. ).o O( thinkin real hard about going away now... )");
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                var thought = string.Join(" ", command.Arguments).Split('\n');
                var thinkerLine = Math.Ceiling(thought.Length / 2.0f);

                var response = "";

                for (var i = 0; i < thought.Length; i++) {
                    string lnUsed = i + 1 == thinkerLine ? thinker : emptyLn;
                    response += string.Format(lnUsed, thought[i]);
                }

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