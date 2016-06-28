using ForecastIO;

namespace Homero.Plugin.Weather
{
    public class WeatherRendererInfo
    {
        public ForecastIOResponse WeatherResponse { get; set; }
        public string Address { get; set; }
        public Unit Unit { get; set; }
    }
}