using System;
using System.Collections.Generic;
using WeatherAggregator.Models;

namespace WeatherAggregator.WeatherProviders
{
    public class OpenWeatherMapProvider : IWeatherProvider
    {
        IObservable<Forecast> IWeatherProvider.GetWeatherForecasts(IEnumerable<City> cities)
        {
            throw new NotImplementedException();
        }
    }
}
