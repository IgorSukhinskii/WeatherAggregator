using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAggregator.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
    }
}