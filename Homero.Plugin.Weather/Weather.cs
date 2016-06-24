﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForecastIO;
using GeocodeSharp.Google;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Messages.Attachments;
using Homero.Services;

namespace Homero.Plugin.Weather
{
    public class Weather : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() {"wea", "weather"};
        private IConfiguration _config;
        private string _geocodeApiKey;
        private string _forecastIoApiKey;

        public Weather(IMessageBroker broker, IConfiguration config)
        {
            _config = config;
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
            if (!_config.Exists)
            {
                _config.SetValue("geocode_api", "AREALAPIKEYNEEDED");
                _config.SetValue("forecast_api", "AREALAPIKEYNEEDED");
                throw new Exception("Forecast.IO API key needed.");
            }

            _geocodeApiKey = _config.GetValue<string>("geocode_api");
            _forecastIoApiKey = _config.GetValue<string>("forecast_api");
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands
        {
            get { return _registeredCommands; }
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;

            string inputLocation = null;
            bool noSave = false, locationValid = false;
            float lat = 0.0f, lng = 0.0f;

            // parse out commandline
            if (e.Command.Arguments.Count > 0)
            {
                inputLocation = String.Join(" ", e.Command.Arguments);
            }
            else
            {
                return;
            }

            if (e.Command.Arguments.Count > 1)
            {
                noSave = e.Command.Arguments[1] == "nosave";
            }

            if (String.IsNullOrEmpty(inputLocation))
            {
                // TODO: lookup location based on username, set locationValid to true if we found one
            }

            GeocodeResult geoResult = null;
            if (!locationValid && inputLocation != null)
            {
                // TODO: migrate to async
                GeocodeResponse geo = _geocode.GeocodeAddress(inputLocation).Result;
                if (geo.Status == GeocodeStatus.Ok)
                {
                    geoResult = geo.Results[0];
                    GeoCoordinate location = geoResult.Geometry.Location;
                    lat = (float)location.Latitude;
                    lng = (float)location.Longitude;
                    locationValid = true;
                }
            }

            if (!locationValid)
            {
                client?.ReplyTo(e.Command,"gotta give me a zipcode or something");
                return;
            }

            string country = geoResult.AddressComponents
                .Where(address => address.Types.Contains("country"))
                .Select(address => address.ShortName)
                .FirstOrDefault();

            Unit unit = country == "US" ? Unit.us : Unit.si;

            ForecastIOResponse weather = new ForecastIORequest(_forecastIoApiKey, lat, lng, unit).Get();

            string summary = $"{geoResult.FormattedAddress} | {weather.currently.summary} | {weather.currently.temperature}{(unit == Unit.us ? "F" : "C")} | Humidity: {weather.currently.humidity * 100}%"
                + $"\n{weather.minutely.summary}";

            if (client?.InlineOrOembedSupported == true)
            {
                WeatherRendererInfo info = new WeatherRendererInfo();
                info.unit = unit;
                info.address = geoResult.FormattedAddress;
                info.weather = weather;
                Stream stream = CreateWeatherImage(info);
                client.ReplyTo(e.Command, new ImageAttachment() {DataStream = stream, Name = $"{e.Command.InnerMessage.Sender} Weather {DateTime.Now}.png"});
            }
            else
            {
                client?.ReplyTo(e.Command, summary);
            }

            // TODO: save to persistent store for username if dontsave isn't specified
        }
    }
}