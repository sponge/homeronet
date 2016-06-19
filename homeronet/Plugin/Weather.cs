using ForecastIO;
using GeocodeSharp.Google;
using homeronet.Client;
using homeronet.Messages;
using Ninject;
using Ninject.Parameters;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System;

namespace homeronet.Plugin {

    public class WeatherRendererInfo {
        public ForecastIOResponse weather;
        public string address;
        public Unit unit;
    }

    public class WeatherRenderer {
        SKTypeface tf = SKTypeface.FromFile("Resources/Weather/Star4000.ttf");
        SKTypeface tfsm = SKTypeface.FromFile("Resources/Weather/Star4000 Small.ttf");
        SKTypeface tflg = SKTypeface.FromFile("Resources/Weather/Star4000 Large.ttf");

        SKColor colorYellow = new SKColor(205, 185, 0);
        SKColor colorOrange = new SKColor(194, 108, 2);
        SKColor colorDkBlue = new SKColor(41, 25, 92);
        SKColor colorBlue = new SKColor(8, 15, 255);
        SKColor colorLtBlue = new SKColor(121, 112, 255);
        SKColor colorTeal = new SKColor(47, 65, 120);
        SKColor colorTealAlso = new SkiaSharp.SKColor(172, 177, 237);
        SKColor colorRed = new SKColor(198, 19, 2);

        private Dictionary<string, string[]> forecastDescriptions = new Dictionary<string, string[]>() {
            {"clear-day", new string[] {"Sunny",""} },
            {"clear-night", new string[] {"Clear",""} },
            {"cloudy", new string[] {"Cloudy",""} },
            {"fog", new string[] {"Fog","" } },
            {"partly-cloudy-day", new string[] {"Partly","Cloudy"} },
            {"partly-cloudy-night", new string[] {"Partly","Cloudy"} },
            {"rain", new string[] {"Rain",""} },
            {"sleet", new string[] {"Sleet",""} },
            {"snow", new string[] {"Snow",""} },
            {"wind", new string[] {"Wind",""} },
        };

        private void DrawForecast(SKCanvas canvas, SKRect rect, ForecastIO.DailyForecast forecast) {
            // background
            using (var paint = new SKPaint()) {
                var colors = new[] { colorLtBlue, colorBlue };

                paint.Color = SKColors.Black;
                canvas.DrawRect(rect, paint);

                rect = new SKRect(rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
                paint.Color = SKColors.Gray;
                canvas.DrawRect(rect, paint);

                rect = new SKRect(rect.Left + 4, rect.Top + 4, rect.Right - 4, rect.Bottom - 4);
                paint.Color = SKColors.Black;
                canvas.DrawRect(rect, paint);

                rect = new SKRect(rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
                var grad = SKBitmap.Decode("Resources/Weather/gradient.png");
                canvas.DrawBitmap(grad, rect);
            }

            var w = (rect.Right - rect.Left);

            // day of week
            using (var paint = new SKPaint()) {
                paint.Color = colorYellow;
                paint.TextSize = 36;
                paint.Typeface = tf;
                paint.TextAlign = SKTextAlign.Center;

                DateTime forecastTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(forecast.time).ToLocalTime();
                DrawShadowedText(canvas, forecastTime.ToString("ddd").ToUpper(), rect.Left + w / 2, rect.Top + 30, paint);
            }

            // forecast icon
            using (var icon = SKBitmap.Decode($"Resources/Weather/{forecast.icon}.gif")) {
                var paddingW = (w - icon.Width) / 2;
                var paddingH = (128 - icon.Height) / 2;
                var imgRect = new SKRect(rect.Left + paddingW, 140 + paddingH, rect.Right - paddingW, 140 + 128 - paddingH);
                canvas.DrawBitmap(icon, imgRect);
            }

            // hi/lo
            using (var paint = new SKPaint()) {
                paint.TextSize = 36;
                paint.Typeface = tf;
                paint.TextAlign = SKTextAlign.Center;

                paint.Color = colorTealAlso;
                DrawShadowedText(canvas, "Lo", rect.Left + w / 4, rect.Bottom - 50, paint);

                paint.Color = colorYellow;
                DrawShadowedText(canvas, "Hi", rect.Left + (w / 4) * 3, rect.Bottom - 50, paint);

                paint.Color = SKColors.White;
                paint.Typeface = tflg;
                paint.TextSize = 32;
                DrawShadowedText(canvas, $"{Math.Round(forecast.temperatureMin)}", rect.Left + w / 4, rect.Bottom - 5, paint);
                DrawShadowedText(canvas, $"{Math.Round(forecast.temperatureMax)}", rect.Left + (w / 4) * 3, rect.Bottom - 5, paint);
            }

            // condition
            using (var paint = new SKPaint()) {
                paint.Color = SKColors.White;
                paint.TextSize = 36;
                paint.Typeface = tf;
                paint.TextAlign = SKTextAlign.Center;

                if (forecastDescriptions.ContainsKey(forecast.icon)) {
                    DrawShadowedText(canvas, forecastDescriptions[forecast.icon][0], rect.Left + w / 2, rect.Bottom - 110 - 50, paint);
                    DrawShadowedText(canvas, forecastDescriptions[forecast.icon][1], rect.Left + w / 2, rect.Bottom - 110, paint);
                }
                else {
                    DrawShadowedText(canvas, forecast.icon, rect.Left + w / 2, rect.Bottom - 90 - 30, paint);
                }
            }
        }

        private void DrawShadowedText(SKCanvas canvas, string text, float x, float y, SKPaint paint) {
            var oldColor = paint.Color;

            paint.Color = SKColors.Black;

            canvas.DrawText(text, x + 2, y + 2, paint);

            paint.IsStroke = true;
            paint.StrokeWidth = 2;
            paint.StrokeJoin = SKStrokeJoin.Bevel;
            canvas.DrawText(text, x, y, paint);
            paint.IsStroke = false;

            paint.Color = oldColor;
            canvas.DrawText(text, x, y, paint);

            paint.Color = oldColor;
        }

        public void DrawWeather(WeatherRendererInfo info, SKCanvas canvas, int width, int height) {

            // background
            using (var paint = new SKPaint()) {
                var colors = new[] { colorDkBlue, colorOrange };

                var headerHeight = 84;

                var shaderGradient = SKShader.CreateLinearGradient(new SKPoint(0, headerHeight), new SKPoint(0, height), colors, null, SKShaderTileMode.Clamp);
                var shaderNoise = SKShader.CreatePerlinNoiseTurbulence(0.99f, 0.99f, 1, 0.0f);
                var shaderComp = SKShader.CreateCompose(shaderGradient, shaderNoise, SKXferMode.Multiply);

                paint.Color = colorDkBlue;
                paint.Shader = shaderComp;

                // background
                canvas.DrawRect(new SKRect(0, 0, width, height), paint);

                // header background
                var grad = SKBitmap.Decode("Resources/Weather/gradient_orange.png");
                canvas.DrawBitmap(grad, new SKRect(0, 0, width, headerHeight));

                // top right cutout
                paint.Shader = null;
                using (var path = new SKPath()) {
                    var baseX = 180;
                    var slant = 80;
                    path.MoveTo(width - baseX, 0);
                    path.LineTo(width, 0);
                    path.LineTo(width, headerHeight);
                    path.LineTo(width - baseX - slant, headerHeight);
                    canvas.DrawPath(path, paint);
                }

            }

            // ticker
            using (var paint = new SKPaint()) {
                var alertsActive = info.weather.alerts?.Count > 0;
                var rect = new SKRect(0, height - 100, width, height);

                paint.Color = SKColors.Black;
                canvas.DrawRect(rect, paint);

                rect.Top += 2;
                paint.Color = SKColors.Gray;
                canvas.DrawRect(rect, paint);

                rect.Top += 2;
                paint.Color = alertsActive ? colorRed : colorTeal;
                canvas.DrawRect(rect, paint);

                paint.Color = SKColors.White;
                paint.TextSize = 36;
                paint.Typeface = tf;
                string message;
                if (alertsActive) {
                    message = String.Join(", ", (from alert in info.weather.alerts select alert.title));
                }
                else {
                    var unit = info.unit == Unit.us ? "F" : "C";
                    message = $"Temp: {(int)info.weather.currently.temperature}°{unit}   Feels Like: {(int)info.weather.currently.apparentTemperature}°{unit}";
                }
                DrawShadowedText(canvas, message, 5, height - 55, paint);

            }


            // logo
            using (var logo = SKBitmap.Decode($"Resources/Weather/logo.png")) {
                var imgRect = new SKRect(20, 5, 20 + logo.Width, 5 + logo.Height); ;
                canvas.DrawBitmap(logo, imgRect);
            }

            // city name and extended forecast header text
            using (var paint = new SKPaint()) {
                paint.TextSize = 36.0f;
                paint.Typeface = tf;
                var baseX = 150.0f;
                var baseY = 32.0f;
                var baseYOff = 30.0f;

                paint.Color = SKColors.White;
                DrawShadowedText(canvas, info.address, baseX, baseY, paint);

                paint.Color = colorYellow;
                DrawShadowedText(canvas, "Extended Forecast", baseX, baseY + baseYOff, paint);
            }

            // top right date and time
            using (var paint = new SKPaint()) {
                // TODO: this is probably not right. i have an offset but no clue what the best way is here
                var now = DateTime.UtcNow + TimeSpan.FromHours(info.weather.offset);

                var topDateLine = now.ToString("h:mm:ss tt").ToUpper();
                var bottomDateLine = now.ToString("ddd MMM d").ToUpper();

                var baseX = width - 280.0f;
                var baseY = 45;

                paint.Typeface = tfsm;
                paint.TextSize = 36;
                paint.Color = SKColors.LightGray;

                DrawShadowedText(canvas, topDateLine, baseX, baseY, paint);
                DrawShadowedText(canvas, bottomDateLine, baseX, baseY + 20, paint);
            }

            for (var i = 0; i < 4; i++) {
                var boxWidth = 220;
                var boxOffset = width / 4;
                DrawForecast(canvas, new SKRect(i * boxOffset + 10, 90, i * boxOffset + boxWidth, height - 90 - 20), info.weather.daily.data[i]);
            }

        }
    }

    public class Weather : IPlugin {
        private GeocodeClient _geocode;
        private IClientConfiguration _forecastConfig;

        private List<string> _registeredCommands = new List<string>()
        {
            "weather"
        };

        private Stream CreateWeatherImage(WeatherRendererInfo info) {
            WeatherRenderer weatherRenderer = new WeatherRenderer();
            int width = 975, height = 575;

            using (var surface = SKSurface.Create(width, height, SKColorType.N_32, SKAlphaType.Opaque)) {
                var skcanvas = surface.Canvas;
                weatherRenderer.DrawWeather(info, skcanvas, width, height);
                var img = surface.Snapshot().Encode();
                return img.AsStream();
            }
        }

        public void Startup() {
            _forecastConfig = Program.Kernel.Get<IClientConfiguration>(new Parameter("ClientName", "Forecast.IO", true));

            var geocodeConfig = Program.Kernel.Get<IClientConfiguration>(new Parameter("ClientName", "GoogleGeocode", true));
            if (geocodeConfig.ApiKey.Length > 0) {
                _geocode = new GeocodeClient(geocodeConfig.ApiKey);
            }
            else {
                _geocode = new GeocodeClient();
            }

            // TODO: initialize persistent store for username -> location mapping
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                string inputLocation = null;
                bool noSave = false, locationValid = false;
                float lat = 0.0f, lng = 0.0f;
                IClient sendingClient = command.InnerMessage.SendingClient;

                // parse out commandline
                if (command.Arguments.Count > 0) {
                    inputLocation = command.Arguments[0];
                }

                if (command.Arguments.Count > 1) {
                    noSave = command.Arguments[1] == "nosave";
                }

                if (inputLocation == null) {
                    // TODO: lookup location based on username, set locationValid to true if we found one
                }

                GeocodeResult geoResult = null;
                if (!locationValid && inputLocation != null) {
                    // TODO: migrate to async
                    var geo = _geocode.GeocodeAddress(inputLocation).Result;
                    if (geo.Status == GeocodeStatus.Ok) {
                        geoResult = geo.Results[0];
                        var location = geoResult.Geometry.Location;
                        lat = (float)location.Latitude;
                        lng = (float)location.Longitude;
                        locationValid = true;
                    }
                }

                if (!locationValid) {
                    return command.InnerMessage.CreateResponse("gotta give me a zipcode or something");
                }

                var country = (from address in geoResult.AddressComponents where address.Types.Contains("country") select address.ShortName).FirstOrDefault();
                var unit = country == "US" ? Unit.us : Unit.si;

                var weather = new ForecastIORequest(_forecastConfig.ApiKey, lat, lng, unit).Get();

                // TODO: create string

                // TODO: create forecast based on discord or irc

                if (sendingClient is DiscordClient) {
                    var info = new WeatherRendererInfo();
                    info.unit = unit;
                    info.address = geoResult.FormattedAddress;
                    info.weather = weather;
                    var stream = CreateWeatherImage(info);

                    // TODO: bad, just duplicating discordclient code
                    // Is it a PM or a public message?
                    var _discordClient = (sendingClient as DiscordClient).RootClient;
                    var message = command.InnerMessage;
                    if (message.IsPrivate) {
                        var targetChannel = _discordClient.PrivateChannels.FirstOrDefault(x => x.Name == message.Channel);
                        var sendMessage = targetChannel?.SendFile("weather.png", stream).Result;
                    }
                    else {
                        Discord.Channel targetedChannel = (_discordClient.Servers
                            .FirstOrDefault(x => x.Name == message.Server)?.TextChannels)?.FirstOrDefault(x => x.Name == message.Channel);

                        var sendMessage = targetedChannel?.SendFile("weather.png", stream).Result;
                    }
                }

                // TODO: save to persistent store for username if dontsave isn't specified

                sendingClient.SendMessage(command.InnerMessage.CreateResponse("weather goes here"));

                return null;
            });
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}