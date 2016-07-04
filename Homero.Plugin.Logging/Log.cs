using Homero.Core.Services;
using Homero.Plugin.Logging.Context;
using Homero.Plugin.Logging.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Logging
{
    public class Log : IPlugin
    {
        private const string LOG_CONNECTION_STRING = "Data Source =.\\log.sqlite";

        public Log(IMessageBroker broker)
        {
            broker.MessageReceived += Broker_MessageReceived;
        }

        private void Broker_MessageReceived(object sender, Core.EventArgs.MessageEventArgs e)
        {
            using (var context = new LogContext(LOG_CONNECTION_STRING))
            {
                User user;
                user = context.Users.FirstOrDefault(x => x.Name == e.User.Name);
                if (user == null)
                {
                    user = new User() { Name = e.User.Name };
                    context.Users.Add(user);
                }

                context.SaveChangesAsync();
            }
        }

        public List<string> RegisteredTextCommands
        {
            get { return null; }
        }

        public void Shutdown()
        {
        }

        public void Startup()
        {
        }
    }
}