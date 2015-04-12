using WeatherAggregator.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WeatherAggregator.DAL
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext()
            : base("ApplicationDbConnection")
        {
        }

        public DbSet<Forecast> Forecasts { get; set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}