using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAggregator.WeatherProviders
{
    public class OpenWeatherMapProvider : AsyncApiProvider<WeatherResponse>
    {
        protected override string ApiCall
        {
            get { return "http://api.openweathermap.org/data/2.5/forecast?lat={1}&lon={2}&units=metric{0}"; } // {0} должен быть пустой строкой
        }

        protected override string ApiKeyName
        {
            get { return "OpenWeatherMapApiKey"; } // эта строка в конфигурации должна быть равна ""
        }

        protected override async Task<IEnumerable<WeatherResponse>> GetResults(Models.City city)
        {
            var response = await this.GetResponse(city);
            IEnumerable<dynamic> weathers = response.list;
            return weathers.Select(weather =>
            {
                int unixTimeStamp = weather.dt;
                var epoch = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
                var dateTime = epoch.AddSeconds(unixTimeStamp);
                float temp = weather.main.temp;
                float hum = weather.main.humidity;
                return new WeatherResponse
                {
                    Time = dateTime,
                    Temperature = temp,
                    Humidity = hum / 100 // влажность передаётся в процентах, а не в долях от единицы
                };
            });
        }

        protected override Models.Forecast ForecastFromResult(WeatherResponse result, Models.City city, DateTime timeAdded)
        {
            return new Models.Forecast
            {
                TimeAdded = timeAdded,
                Time = result.Time,
                Temperature = result.Temperature,
                Humidity = result.Humidity,
                City = city,
                WeatherProvider = Models.WeatherProviderId.OpenWeatherMap
            };
        }
    }
}
