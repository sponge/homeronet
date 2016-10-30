using Homero.Core;
using Homero.Core.Services;
using Homero.Plugin.Logging.Context;
using Homero.Plugin.Logging.Entity;
using System;
using System.Collections.Generic;

namespace Homero.Plugin.Logging
{
    public class Log : IPlugin
    {
        private Dictionary<string, LogContext> _contextStorage; // This is probably not so good....

        public Log(IMessageBroker broker)
        {
            _contextStorage = new Dictionary<string, LogContext>();
            broker.MessageReceived += Broker_MessageReceived;
        }

        private void Broker_MessageReceived(object sender, Core.EventArgs.MessageEventArgs e)
        {
            IClient client = sender as IClient;
            if (sender == null)
            {
                return;
            }
            string contextName;
            LogContext context;

            if (e.Server.Name == null)
            {
                contextName = $"{client.Name}.PM";
                context = new LogContext(contextName);
            }
            else
            {
                contextName = $"{client.Name}.{e.Server.Name}";
                if (!_contextStorage.ContainsKey(contextName))
                {
                    // Cache the channel contexts. They're important and mostly static.
                    _contextStorage[contextName] = new LogContext(contextName);
                }
                context = _contextStorage[contextName];
            }

            Message message = new Message
            {
                Timestamp = DateTime.Now,
                Channel = e.Channel.Name,
                Content = e.Message.Message,
                User = e.User.Name
            };
            context.Messages.Add(message);
            context.SaveChangesAsync();
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