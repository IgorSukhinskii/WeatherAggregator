using System;
using System.Collections.Generic;

namespace WeatherAggregator.Models
{
    public class ForecastVM
    {
        public float TempMin { get; set; }
        public float TempMax { get; set; }
        public float HumidityMin { get; set; }
        public float HumidityMax { get; set; }
    }

    public class DayForecastsVM
    {
        public DateTime Date { get; set; }
        public IDictionary<TimeOfDay, ForecastVM> Forecasts { get; set; }
    }

    public class CityForecastsVM
    {
        public City City { get; set; }
        public IEnumerable<DayForecastsVM> Days { get; set; }
    }
}
