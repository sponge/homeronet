using Homero.Core.Services;
using Homero.Plugin.Logging.Context;
using Homero.Plugin.Logging.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Client;

namespace Homero.Plugin.Logging
{
    public class Log : IPlugin
    {
        private const string LOG_CONNECTION_STRING = "Data Source =.\\{0}.sqlite";
        private Dictionary<string, LogContext> _contextStorage; // This is probably not so good....

        public Log(IMessageBroker broker)
        {
            broker.MessageReceived += Broker_MessageReceived;
        }

        private void Broker_MessageReceived(object sender, Core.EventArgs.MessageEventArgs e)
        {
            IClient client = sender as IClient;
            if (sender == null)
            {
                return;
            }

            string contextName = $"{client.Name}.{e.Server.Name}";

            if (!_contextStorage.ContainsKey(contextName))
            {
                _contextStorage[contextName] = new LogContext(string.Format(LOG_CONNECTION_STRING, contextName));
            }

            Message message = new Message();
            message.Timestamp = DateTime.Now;
            message.Channel = e.Channel.Name;
            message.Content = e.Message.Message;
            message.User = e.User.Name;
            _contextStorage[contextName].Messages.Add(message);
            _contextStorage[contextName].SaveChangesAsync();
        }

        public List<string> RegisteredTextCommands => null;

        public
            void Shutdown()
        {
        }

        public
            void Startup()
        {
        }
    }
}