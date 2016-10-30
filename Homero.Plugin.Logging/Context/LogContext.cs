using Homero.Core.Database.Context;
using Homero.Plugin.Logging.Entity;
using System.Data.Entity;

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