using ForecastIO;
using Geocoding.Google;
using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages.Attachments;
using Homero.Core.Services;
using Homero.Plugin.Weather.Renderer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Homero.Core.Interface;

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

        public List<string> RegisteredTextCommands { get; } = new List<string> { "weather" };

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var task = Task.Run(async () =>
            {
                string inputLocation = null;
                bool noSave = false;
                // parse out commandline
                if (e.Command.Arguments.Count > 0)
                {
                    inputLocation = string.Join(" ", e.Command.Arguments.Where(x => x != "nosave"));
                }
                if (e.Command.Arguments.Count > 1)
                {
                    noSave = e.Command.Arguments.Any(x => x == "nosave");
                }

                string userAddress = null;
                Tuple<float, float> location = null;
                bool isMetric = false;

                if (string.IsNullOrEmpty(inputLocation))
                {
                    using (var ctx = new UserContext("weather"))
                    {
                        // TODO: Don't break in PMs on discord
                        WeatherUser user =
                            ctx.Users.FirstOrDefault(
                                x => x.Server == e.Server.Name && x.Client == ((IClient) sender).Name &&
                                     x.Username == e.User.Name);

                        userAddress = user?.Address;
                        if (user != null)
                        {
                            location = new Tuple<float, float>(user.Latitude, user.Longitude);
                        }
                    }
                    if (userAddress == null)
                    {
                        e.ReplyTarget.Send("gotta give me a zipcode or something");
                        return;
                    }
                }

                if (userAddress == null)
                {
                    IEnumerable<GoogleAddress> addresses = await _geocode.GeocodeAsync(inputLocation);

                    if (addresses != null)
                    {
                        var address = addresses.FirstOrDefault(x => !x.IsPartialMatch);
                        userAddress = address?.FormattedAddress;
                        if (userAddress != null)
                        {
                            isMetric = address[GoogleAddressType.Country].ShortName != "US";
                            location = new Tuple<float, float>((float) address.Coordinates.Latitude,
                                (float) address.Coordinates.Longitude);
                            if (!noSave)
                            {
                                using (var ctx = new UserContext("weather"))
                                {
                                    WeatherUser user =
                                        ctx.Users.FirstOrDefault(
                                            x =>
                                                x.Server == e.Server.Name && x.Client == ((IClient) sender).Name &&
                                                x.Username == e.User.Name) ?? new WeatherUser();
                                    user.Server = e.Server.Name;
                                    user.Client = ((IClient) sender).Name;
                                    user.Username = e.User.Name;
                                    user.Address = userAddress;
                                    user.Latitude = location.Item1;
                                    user.Longitude = location.Item2;
                                    user.IsMetric = isMetric;
                                    ctx.Users.AddOrUpdate(user);
                                    await ctx.SaveChangesAsync();
                                }
                            }
                        }
                        else
                        {
                            e.ReplyTarget.Send($"where in the bloody hell is {inputLocation}?");
                            return;
                        }
                    }
                }

                var unit = isMetric ? Unit.si : Unit.us;
                var weather = new ForecastIORequest(_forecastIoApiKey, location.Item1, location.Item2, unit).Get();

                string summary = $"{userAddress} | {weather.currently.summary} | {weather.currently.temperature}{(unit == Unit.us ? "F" : "C")} | Humidity: {weather.currently.humidity*100}%"
                                 + $"\n{weather.minutely.summary}";

                var info = new WeatherRendererInfo();
                info.Unit = unit;
                info.Address = userAddress;
                info.WeatherResponse = weather;
                var stream = CreateWeatherImage(info);
                e.ReplyTarget.Send(summary,
                    new ImageAttachment
                    {
                        DataStream = stream,
                        Name = $"{e.User.Name} Weather {DateTime.Now}.png"
                    });

            });

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerExceptions.Last();
            }

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