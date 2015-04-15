using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using WeatherAggregator.Models;

namespace WeatherAggregator.WeatherProviders
{
    public abstract class SyncApiProvider<ApiResultT> : IWeatherProvider
    {
        // абстрактный класс, реализующий метод получения потока прогнозов через синхронное API
        // для полной реализации необходимо переопределить методы GetResults и ForecastFromResult

        // метод для получения прогноза через API (синхронный)
        protected abstract IEnumerable<ApiResultT> GetResults(City city);

        // метод преобразования одного результата API в Forecast
        protected abstract Forecast ForecastFromResult(ApiResultT result, City city, DateTime timeAdded);

        IObservable<Forecast> IWeatherProvider.GetWeatherForecasts(IEnumerable<City> cities)
        {
            var timeAdded = DateTime.UtcNow;
            // для каждого города создадим асинхронную задачу, выполняющую запрос к ForecastIO и возвращающую результат
            var tasks = cities.Select(city =>
                // этот класс предназначается для синхронных API, поэтому используем Task.Run()
                Task.Run(() =>
                    // поскольку запросы могут приходить не в том порядке, в котором мы их послали, мы добавим к результату,
                    // пришедшему от сервера, город. таким образом мы его не потеряем когда будем преобразовывать результаты
                    Tuple.Create(GetResults(city), city)));

            return tasks.Select(t => t.ToObservable())
                .Merge()
                // после превращения каждой асинхронной задачи в Observable, Merge добавит их в результирующий поток
                // в порядке выполнения
                // каждый результат запроса даёт множество прогнозов, поэтому используем SelectMany для того, чтобы преобразовать
                // каждый из них в Forecast и "сплющить" результирующие последовательности в один поток Observable
                .SelectMany(response => response.Item1.Select(result => this.ForecastFromResult(result, response.Item2, timeAdded)));
        }
    }
}
