using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAggregator.Models
{
    public class Forecast
    {
        public int ForecastId { get; set; }
        public DateTime TimeAdded { get; set; }
        public DateTime Time { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public City City { get; set; }
    }
}