using Homero.Core.Database.Configuration;
using SQLite.CodeFirst;
using System.Data.Entity;

namespace Homero.Core.Database.Context
{
    [DbConfigurationType(typeof(SQLiteConfiguration))]
    public abstract class SqliteDbContext<T> : DbContext where T : DbContext
    {
        protected SqliteDbContext(string name) : base(name)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SqliteCreateDatabaseIfNotExists<T> sqlConnectionInitializer = new SqliteCreateDatabaseIfNotExists<T>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqlConnectionInitializer);
        }
    }
}