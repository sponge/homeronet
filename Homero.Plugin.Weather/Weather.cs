using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ForecastIO;
using Geocoding;
using Geocoding.Google;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;
using Homero.Plugin.Weather.Renderer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Homero.Plugin.Weather
{
    public class Weather : IPlugin
    {
        private IConfiguration _config;
        private string _forecastIoApiKey;

        private GoogleGeocoder _geocode;
        private string _geocodeApiKey;

        public Weather(IMessageBroker broker, IConfiguration config)
        {
            _config = config;
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
            if (!_config.Exists)
            {
                _config.SetValue("geocode_api", "");
                _config.SetValue("forecast_api", "");
                throw new Exception("Forecast.IO API key needed.");
            }

            _geocodeApiKey = _config.GetValue<string>("geocode_api");
            _forecastIoApiKey = _config.GetValue<string>("forecast_api");
            if (string.IsNullOrEmpty(_geocodeApiKey))
            {
                _geocode = new GoogleGeocoder();
            }
            else
            {
                _geocode = new GoogleGeocoder() { ApiKey = _geocodeApiKey };
            }
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "wea", "weather" };

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            string inputLocation = null;
            bool noSave = false, locationValid = false;
            float lat = 0.0f, lng = 0.0f;

            // parse out commandline
            if (e.Command.Arguments.Count > 0)
            {
                inputLocation = string.Join(" ", e.Command.Arguments);
            }
            else
            {
                return;
            }

            if (e.Command.Arguments.Count > 1)
            {
                noSave = e.Command.Arguments[1] == "nosave";
            }

            if (string.IsNullOrEmpty(inputLocation))
            {
                // TODO: lookup location based on username, set locationValid to true if we found one
            }

            GoogleAddress address = null;
            if (!locationValid && inputLocation != null)
            {
                // TODO: migrate to async
                IEnumerable<GoogleAddress> addresses = _geocode.Geocode(inputLocation).Where(x => !x.IsPartialMatch);
                if (addresses.Count() > 0)
                {

                    address = addresses.First();
                    lat = (float)address.Coordinates.Latitude;
                    lng = (float)address.Coordinates.Longitude;
                    locationValid = true;
                }
            }

            if (!locationValid)
            {
                e.ReplyTarget.Send("gotta give me a zipcode or something");
                return;
            }

            string country = address[GoogleAddressType.Country].ShortName;

            var unit = country == "US" ? Unit.us : Unit.si;

            var weather = new ForecastIORequest(_forecastIoApiKey, lat, lng, unit).Get();


            string summary = $"{address.FormattedAddress} | {weather.currently.summary} | {weather.currently.temperature}{(unit == Unit.us ? "F" : "C")} | Humidity: {weather.currently.humidity * 100}%"
                + $"\n{weather.minutely.summary}";


                var info = new WeatherRendererInfo();
                info.Unit = unit;
                info.Address = address.FormattedAddress;
                info.WeatherResponse = weather;
                var stream = CreateWeatherImage(info);
                e.ReplyTarget.Send(summary,
                    new ImageAttachment
                    {
                        DataStream = stream,
                        Name = $"{e.User.Name} Weather {DateTime.Now}.png"
                    });

            // TODO: save to persistent store for username if dontsave isn't specified
        }

        private Stream CreateWeatherImage(WeatherRendererInfo info)
        {
            var weatherRenderer = new WeatherRenderer();
            int width = 975, height = 575;

            using (var surface = SKSurface.Create(width, height, SKColorType.N_32, SKAlphaType.Opaque))
            {
                var skcanvas = surface.Canvas;
                weatherRenderer.DrawWeather(info, skcanvas, width, height);
                var img = surface.Snapshot().Encode();
                return img.AsStream();
            }
        }
    }
}