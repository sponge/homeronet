using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;

namespace Homero.Core.Database.Provider
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private const string LOG_CONNECTION_STRING = "Data Source =.\\Data\\{0}.sqlite";

        public DbConnection CreateConnection(string dbFilename)
        {
            return new SQLiteConnection(string.Format(LOG_CONNECTION_STRING, dbFilename));
        }
    }
}