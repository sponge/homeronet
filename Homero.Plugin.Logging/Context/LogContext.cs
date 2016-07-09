using Homero.Plugin.Logging.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Database.Context;

namespace Homero.Plugin.Logging.Context
{
    public class LogContext : SqliteDbContext<LogContext>
    {
        public LogContext(string name) : base(name)
        {
        }

        public DbSet<Message> Messages { get; set; }
    }
}