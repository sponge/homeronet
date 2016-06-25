using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ForecastIO;
using GeocodeSharp.Google;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;
using Homero.Plugin.Weather.Renderer;
using SkiaSharp;

namespace Homero.Plugin.Weather
{
    public class Weather : IPlugin
    {
        private IConfiguration _config;
        private string _forecastIoApiKey;

        private GeocodeClient _geocode;
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
                _geocode = new GeocodeClient();
            }
            else
            {
                _geocode = new GeocodeClient(_geocodeApiKey);
            }
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> {"wea", "weather"};

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

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

            GeocodeResult geoResult = null;
            if (!locationValid && inputLocation != null)
            {
                // TODO: migrate to async
                var geo = _geocode.GeocodeAddress(inputLocation).Result;
                if (geo.Status == GeocodeStatus.Ok)
                {
                    geoResult = geo.Results[0];
                    var location = geoResult.Geometry.Location;
                    lat = (float) location.Latitude;
                    lng = (float) location.Longitude;
                    locationValid = true;
                }
            }

            if (!locationValid)
            {
                client?.ReplyTo(e.Command, "gotta give me a zipcode or something");
                return;
            }

            var country = geoResult.AddressComponents
                .Where(address => address.Types.Contains("country"))
                .Select(address => address.ShortName)
                .FirstOrDefault();

            var unit = country == "US" ? Unit.us : Unit.si;

            var weather = new ForecastIORequest(_forecastIoApiKey, lat, lng, unit).Get();

            var summary = $"{geoResult.FormattedAddress} | {weather.currently.summary} | {weather.currently.temperature}{(unit == Unit.us ? "F" : "C")} | Humidity: {weather.currently.humidity*100}%"
                          + $"\n{weather.minutely.summary}";

            if (client?.InlineOrOembedSupported == true)
            {
                var info = new WeatherRendererInfo();
                info.Unit = unit;
                info.Address = geoResult.FormattedAddress;
                info.WeatherResponse = weather;
                var stream = CreateWeatherImage(info);
                client.ReplyTo(e.Command,
                    new ImageAttachment
                    {
                        DataStream = stream,
                        Name = $"{e.Command.InnerMessage.Sender} Weather {DateTime.Now}.png"
                    });
            }
            else
            {
                client?.ReplyTo(e.Command, summary);
            }

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