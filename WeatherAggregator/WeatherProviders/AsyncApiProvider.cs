using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using WeatherAggregator.Models;

namespace WeatherAggregator.WeatherProviders
{
    /// <summary>
    /// абстрактный класс, реализующий метод получения потока прогнозов через асинхронное API
    /// для полной реализации необходимо переопределить методы GetResults и ForecastFromResult
    /// </summary>
    /// <typeparam name="ApiResultT">Тип, соответствующий одному прогнозу в API</typeparam>
    public abstract class AsyncApiProvider<ApiResultT> : IWeatherProvider
    {
        /// <summary>
        /// строка, с помощью которой формируется строка запроса к API
        /// </summary>
        protected abstract string ApiCall { get; }

        /// <summary>
        /// Имя ключа для API в настройках
        /// </summary>
        protected abstract string ApiKeyName { get; }

        /// <summary>
        /// Метод для получения результата от сервера в максимально сыром виде
        /// </summary>
        /// <param name="city">Город, для которого осуществляется запрос</param>
        /// <returns>Результирующий JSON-объект в виде dynamic</returns>
        protected async Task<dynamic> GetResponse(City city)
        {
            var culture = CultureInfo.InvariantCulture;
            var latStr = city.Lat.ToString(culture);
            var lngStr = city.Lng.ToString(culture);
            var uri = string.Format(ApiCall, ConfigurationManager.AppSettings[ApiKeyName], latStr, lngStr);
            using (var client = new HttpClient())
            {
                var httpResponse = await client.GetAsync(uri);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var content = await httpResponse.Content.ReadAsStringAsync();
                    dynamic result = JObject.Parse(content);
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// метод для получения прогноза через API (асинхронный)
        /// </summary>
        /// <param name="city">Город, для которого необходимо получить прогнозы</param>
        /// <returns>Возвращает асинхронную задачу, результатом которой будет последовательность прогнозов</returns>
        protected abstract Task<IEnumerable<ApiResultT>> GetResults(City city);

        /// <summary>
        /// Метод преобразования одного прогноза из API в модель Forecast
        /// </summary>
        /// <param name="result">Прогноз, который необходимо преобразовать в Forecast</param>
        /// <param name="city">Город, для которого справедлив этот прогноз</param>
        /// <param name="timeAdded">Время, когда прогноз был сделан</param>
        /// <returns></returns>
        protected abstract Forecast ForecastFromResult(ApiResultT result, City city, DateTime timeAdded);

        IObservable<Forecast> IWeatherProvider.GetWeatherForecasts(IEnumerable<City> cities)
        {
            var timeAdded = DateTime.UtcNow;
            return cities
                .Select(city =>
                    this.GetResults(city) // для каждого города запустим асинхронную задачу на получение прогноза
                        .ToObservable()   // превратим её в Observable
                        .Select(results => Tuple.Create(results, city))) // и завернём результат в кортеж с городом
                .Merge()
                // после превращения каждой асинхронной задачи в Observable, Merge добавит их в результирующий поток
                // в порядке выполнения
                // каждый результат запроса даёт множество прогнозов, поэтому используем SelectMany для того, чтобы преобразовать
                // каждый из них в Forecast и "сплющить" результирующие последовательности в один поток Observable
                .SelectMany(response => response.Item1.Select(result => this.ForecastFromResult(result, response.Item2, timeAdded)));
        }
    }
}
