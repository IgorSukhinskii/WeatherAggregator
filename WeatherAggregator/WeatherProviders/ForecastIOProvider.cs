using ForecastIO;
using ForecastIO.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using WeatherAggregator.Models;

namespace WeatherAggregator.WeatherProviders
{
    public class ForecastIOProvider : IWeatherProvider
    {
        IObservable<Forecast> IWeatherProvider.GetWeatherForecasts(IEnumerable<City> cities)
        {
            var timeAdded = DateTime.UtcNow;
            var tasks = cities.Select(city =>
                // для каждого города создадим асинхронную задачу, выполняющую запрос к ForecastIO и возвращающую результат
                Task.Run(() =>
                    // поскольку запросы могут приходить не в том порядке, в котором мы их послали, мы добавим к результату, пришедшему
                    // от сервера, город. таким образом мы его не потеряем когда будем преобразовывать результаты
                    // ForecastIORequest.Get() -- синхронный метод, поэтому используем Task.Run()
                    Tuple.Create(new ForecastIORequest("74877fab479f3c31427b0d5478b6a353", city.Lat, city.Lng, Unit.si).Get(),
                                 city)));
            return tasks.Select(t => t.ToObservable())
                        .Merge() // после превращения каждой асинхронной задачи в Observable, Merge добавит их в результирующий поток
                        // в порядке выполнения
                        // каждый результат запроса даёт множество прогнозов, поэтому используем SelectMany для того, чтобы преобразовать
                        // каждый из них в Forecast и "сплющить" результирующие последовательности в один поток Observable
                        .SelectMany(response => response.Item1.hourly.data.Select(hourForecast =>
                            new Forecast {
                                Time = hourForecast.time.ToDateTime(),
                                TimeAdded = timeAdded,
                                Temperature = hourForecast.temperature,
                                Humidity = hourForecast.humidity,
                                City = response.Item2,
                                WeatherProvider = WeatherProviderId.ForecastIO
                            }).Where(forecast => forecast.City != null));
        }
    }
}
