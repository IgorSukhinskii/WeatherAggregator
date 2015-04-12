using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WeatherAggregator.Models;

namespace WeatherAggregator.DAL
{
    class ApplicationDbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var cities = new List<City>()
            {
                new City{Name="Челябинск", Lat=55.154442f, Lng=61.429722f},
                new City{Name="Москва", Lat=55.75222f, Lng=37.615555f}
            };
            cities.ForEach(c => context.Cities.Add(c));
            context.SaveChanges();
        }
    }
}