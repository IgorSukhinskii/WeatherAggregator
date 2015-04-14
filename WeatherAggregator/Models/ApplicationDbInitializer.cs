using System.Collections.Generic;
using WeatherAggregator.Models;

namespace WeatherAggregator.DAL
{
    public class ApplicationDbInitializer : System.Data.Entity.DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var cities = new List<City>()
            {
                new City{Name="Челябинск", Lat=55.154442f, Lng=61.429722f},
                new City{Name="Москва", Lat=55.75222f, Lng=37.615555f},
                new City{Name="Нью-Йорк", Lat=40.71427f, Lng=-74.00597f}
            };
            cities.ForEach(c => context.Cities.Add(c));
            context.SaveChanges();
        }
    }
}