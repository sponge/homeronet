using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Database.Context;

namespace Homero.Plugin.Weather
{
    public class UserContext : SqliteDbContext<UserContext>
    {
        public UserContext(string name) : base(name)
        {
        }

        public DbSet<WeatherUser> Users { get; set; }
    }
}
