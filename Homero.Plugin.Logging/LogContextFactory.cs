using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Client;
using Homero.Core.Interface;
using Homero.Plugin.Logging.Context;

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
