using System.Web.Mvc;
using WeatherAggregator.DAL;
namespace WeatherAggregator.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext dbContext = new ApplicationDbContext();
    }
}