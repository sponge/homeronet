using homeronet.Messages;
using homeronet.Plugins;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace homeronet.plugins
{
    public class Homero : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() { "homero" };

        public void Startup()

        {
            Program.Log.Info("I startup, ola.");
        }

        public void Shutdown()
        {
        }

        public Task<IStandardMessage> HandleTextCommandInvocationAsync(ITextCommand command)
        {
            return new Task<IStandardMessage>(() =>
            {
                Debug.WriteLine("Homero got a message.");

                return null;
            });
        }

        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (message.Message == "hello homero")
                {
                    return new StandardMessage()
                    {
                        Target = message.Sender,
                        IsPrivate = message.IsPrivate,
                        Channel = message.Channel,
                        Message = "hi friend",
                        Server = message.Server
                    };
                }
                return null;
            });
        }
    }
}