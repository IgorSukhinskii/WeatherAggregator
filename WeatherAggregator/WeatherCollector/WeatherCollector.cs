using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using WeatherAggregator.DAL;
using WeatherAggregator.WeatherProviders;

namespace WeatherAggregator.WeatherCollector
{
    public static class WeatherCollector
    {
        private static IEnumerable<IWeatherProvider> providers = new List<IWeatherProvider>
        {
            new ForecastIOProvider()
        };

        public static void Collect()
        {
            var context = new ApplicationDbContext();
            var cities = context.Cities.ToList();
            providers.Select(provider => provider.GetWeatherForecasts(cities)) // преобразуем провайдеров в потоки прогнозов
                     .Merge()                                                  // соединим все потоки прогнозов в один
                     .Subscribe(new WriteDbForecastsObserver(context));        // подпишем наблюдателя, который запишет каждый прогноз в бд
        }
    }
}
