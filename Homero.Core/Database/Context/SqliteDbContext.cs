using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Database.Configuration;
using SQLite.CodeFirst;

namespace Homero.Core.Database.Context
{
    [DbConfigurationType(typeof(SQLiteConfiguration))]
    public abstract class SqliteDbContext<T> : DbContext where T : DbContext
    {
        protected SqliteDbContext(string name) :base(name)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SqliteCreateDatabaseIfNotExists<T> sqlConnectionInitializer = new SqliteCreateDatabaseIfNotExists<T> (modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqlConnectionInitializer);
        }
    }


}
