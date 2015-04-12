using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForecastIO;

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
    }
}