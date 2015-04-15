using ForecastIO;
using ForecastIO.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using WeatherAggregator.Models;

namespace WeatherAggregator.WeatherProviders
{
    public class ForecastIOProvider : SyncApiProvider<HourForecast>
    {
        protected override IEnumerable<HourForecast> GetResults(City city)
        {
            return new ForecastIORequest(ConfigurationManager.AppSettings["ForecastIOApiKey"], city.Lat, city.Lng, Unit.si).Get().hourly.data;
        }

        protected override Forecast ForecastFromResult(HourForecast result, City city, DateTime timeAdded)
        {
            return new Forecast
            {
                Time = result.time.ToDateTime(),
                TimeAdded = timeAdded,
                Temperature = result.temperature,
                Humidity = result.humidity,
                City = city,
                WeatherProvider = WeatherProviderId.ForecastIO
            };
        }
    }
}
