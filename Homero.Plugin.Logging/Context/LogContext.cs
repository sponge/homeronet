﻿using Homero.Plugin.Logging.Entity;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Logging.Context
{
    [DbConfigurationType(typeof(SQLiteConfiguration))]
    public class LogContext : DbContext
    {
        public LogContext()
        {
        }

        public LogContext(string name) : base(name)
        {
        }

        public DbSet<Server> Servers { get; set; }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SqliteCreateDatabaseIfNotExists<LogContext> sqlConnectionInitializer = new SqliteCreateDatabaseIfNotExists<LogContext>(modelBuilder);
            Database.SetInitializer(sqlConnectionInitializer);
        }
    }
}