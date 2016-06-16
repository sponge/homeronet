using homeronet.Messages;
using homeronet.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace homeronet.plugins
{
    public class Homero : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() { "homero" };

        public void Startup()

        {
        }

        public void Shutdown()
        {
        }

        public Task<IStandardMessage> HandleTextCommandInvocationAsync(ITextCommand command)
        {
            return new Task<IStandardMessage>(() =>
            {
                return null;
            });
        }

        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

        public Task ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}