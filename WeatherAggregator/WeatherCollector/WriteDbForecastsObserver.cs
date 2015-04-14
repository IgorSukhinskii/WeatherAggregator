using System;
using WeatherAggregator.DAL;
using WeatherAggregator.Models;

namespace WeatherAggregator.WeatherCollector
{
    public class WriteDbForecastsObserver : IObserver<Forecast>
    {
        private ApplicationDbContext context;
        public WriteDbForecastsObserver(ApplicationDbContext context)
        {
            this.context = context;
        }

        void IObserver<Forecast>.OnCompleted()
        {
            context.SaveChanges();
        }

        void IObserver<Forecast>.OnError(Exception error)
        {
            throw error;
        }

        void IObserver<Forecast>.OnNext(Forecast value)
        {
            context.Forecasts.Add(value);
        }
    }
}
