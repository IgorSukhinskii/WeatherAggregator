using System;

namespace WeatherAggregator.Models
{
    public enum WeatherProviderId
    {
        ForecastIO,
        OpenWeatherMap
    }
    public class Forecast
    {
        public int ForecastId { get; set; }
        public DateTime TimeAdded { get; set; }
        public DateTime Time { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public WeatherProviderId WeatherProvider { get; set; }
    }
}