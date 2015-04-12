using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeatherAggregator.Startup))]
namespace WeatherAggregator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
