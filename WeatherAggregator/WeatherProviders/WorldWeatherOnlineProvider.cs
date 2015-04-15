using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Globalization;

namespace WeatherAggregator.WeatherProviders
{
    public class WeatherResponse
    {
        public DateTime Time { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
    }

    public class WorldWeatherOnlineProvider : AsyncApiProvider<WeatherResponse>
    {
        protected override string ApiCall
        {
            get { return "http://api.worldweatheronline.com/free/v2/weather.ashx?key={0}&q={1},{2}&format=json"; }
        }

        protected override string ApiKeyName
        {
            get { return "WorldWeatherOnlineApiKey"; }
        }

        protected override async Task<IEnumerable<WeatherResponse>> GetResults(Models.City city)
        {
            var response = await this.GetResponse(city);
            IEnumerable<dynamic> weathers = response.data.weather;
            return weathers.SelectMany(weather =>
            {
                IEnumerable<dynamic> hourlyData = weather.hourly;
                return hourlyData.Select(hourly =>
                {
                    // дата передаётся в формате "2015-03-21"
                    string dateStr = weather.date;
                    var dateList = dateStr
                        .Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(Int32.Parse)
                        .ToList();
                    //время передаётся в формате "200" или "1530", где 2, 15 -- часы; 00, 30 -- минуты
                    string strTime = hourly.time; // dynamic это весело
                    var time = Int32.Parse(strTime);
                    var dateTime = new DateTime(dateList[0], dateList[1], dateList[2], time / 100, time % 100, 0, DateTimeKind.Utc);
                    return new WeatherResponse
                    {
                        Time = dateTime,
                        Temperature = (float)Double.Parse((string)(hourly.tempC)),
                        Humidity = (float)Double.Parse((string)(hourly.humidity)) / 100 // влажность передаётся в процентах, а не в долях от единицы
                    };
                });
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
                WeatherProvider = Models.WeatherProviderId.WorldWeatherOnline
            };
        }
    }
}
