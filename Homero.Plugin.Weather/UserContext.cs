using Homero.Core.Database.Context;
using System.Data.Entity;

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