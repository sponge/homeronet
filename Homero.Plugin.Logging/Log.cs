using Homero.Plugin.Logging.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Logging
{
    public class Log : IPlugin
    {
        public Log()
        {
            using (var context = new LogContext())
            {
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