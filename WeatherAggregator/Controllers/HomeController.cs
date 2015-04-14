using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WeatherAggregator.Models;

namespace WeatherAggregator.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View(dbContext.Cities.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Forecasts(int Id)
        {
            var today = DateTime.UtcNow.Date;
            var lastDay = today.AddDays(3);
            var city = dbContext.Cities.Find(Id);
            if (city == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "Город с таким идентификатором не найден");
            }
            // сначала получим все прогнозы для данного города, актуальные на ближайшую неделю
            List<DayForecastsVM> relevantForecasts = dbContext.Forecasts
                .Where(f => f.Time >= today &&
                            f.Time < lastDay &&
                            f.City.CityId == city.CityId)
                .GroupBy(f => new
                {
                    Time = DbFunctions.AddHours(DbFunctions.TruncateTime(f.Time), f.Time.Hour),
                    Provider = f.WeatherProvider
                }).ToList() // дальше встречаются методы, которые сложно транслировать в sql, поэтому загрузим
                // промежуточный результат в память через .ToList()
                // выбираем из всех прогнозов на один и тот же час наиболее свежий
                .Select(g => g.Aggregate((f1, f2) => f1.TimeAdded > f2.TimeAdded ? f1 : f2))
                // группа верхнего уровня -- день недели, для которого актуален прогноз
                .OrderBy(f => f.Time) // дни недели должны идти по порядку
                .GroupBy(f => f.Time.Date)
                .Select(g => new DayForecastsVM
                {
                    Date = g.Key,
                    Forecasts = g.GroupBy(f => f.Time.Date.AddHours(f.Time.Hour)) // сгруппируем по часам
                        // для каждого часа посчитаем средние значение температуры и влажности
                        // благодаря предыдущим группировкам, в кажом часу будет не более чем по одному прогнозу от каждого провайдера
                        .Select(g1 => new
                        {
                            AvgTemp = g1.Average(f => f.Temperature),
                            AvgHum = g1.Average(f => f.Humidity),
                            Time = g1.Key
                        })
                        // сгруппируем по времени суток
                        .GroupBy(f => TimeOfDayHelper.FromDateTime(f.Time))
                        // и для каждого времени суток вычислим интервал температуры и влажности
                        .Select(g1 => new
                        {
                            ToD = g1.Key,
                            Data = new ForecastVM
                            {
                                TempMax = g1.Max(f => f.AvgTemp),
                                TempMin = g1.Min(f => f.AvgTemp),
                                HumidityMax = g1.Max(f => f.AvgHum),
                                HumidityMin = g1.Min(f => f.AvgHum),
                            }
                        })
                        .ToDictionary(o => o.ToD, o => o.Data)
                }).ToList();
            if (relevantForecasts.Count < 3 ||
                relevantForecasts[1].Forecasts.Count < 4)
            {
                // если данных было собрано недостаточно или они были собраны давно, то запустим сбор заново
                // это должен был делать планировщик ежедневно, но, к сожалению, мне не удалось его заставить работать
                // на своём компьютере
                WeatherCollector.WeatherCollector.Collect();
            }
            return View(new CityForecastsVM { City = city, Days = relevantForecasts });
        }
    }
}