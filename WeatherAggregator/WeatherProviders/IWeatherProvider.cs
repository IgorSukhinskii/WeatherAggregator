using System;
using System.Collections.Generic;
using WeatherAggregator.Models;

namespace WeatherAggregator.WeatherProviders
{
    interface IWeatherProvider
    {
        // по сути, IWeatherProvider -- это абстрактная фабрика потоков прогнозов погоды
        // реализация данного метода определяет, каким образом будет получен прогноз погоды,
        // и возвращать Observable, который оповещает наблюдателей каждый раз при получении нового прогноза
        IObservable<Forecast> GetWeatherForecasts(IEnumerable<City> cities);
    }
}
