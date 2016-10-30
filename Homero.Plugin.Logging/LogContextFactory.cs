using Homero.Core;
using Homero.Plugin.Logging.Context;
using System;

namespace Homero.Plugin.Logging
{
    public static class LogContextFactory
    {
        public static LogContext Get(IClient client, IServer server)
        {
            try
            {
                LogContext context = new LogContext($"{client.Name}.{server.Name}");
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                return context;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}