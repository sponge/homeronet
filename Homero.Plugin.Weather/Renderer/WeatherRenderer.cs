using System;
using System.Linq;
using ForecastIO;
using Homero.Core.Utility;
using SkiaSharp;

namespace Homero.Plugin.Weather.Renderer
{
    public class WeatherRenderer
    {
        private void DrawForecast(SKCanvas canvas, SKRect rect, DailyForecast forecast)
        {
            // background
            using (var paint = new SKPaint())
            {
                var colors = new[] {WeatherColors.LightBlue, WeatherColors.Blue};

                paint.Color = SKColors.Black;
                canvas.DrawRect(rect, paint);

                rect = new SKRect(rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
                paint.Color = SKColors.Gray;
                canvas.DrawRect(rect, paint);

                rect = new SKRect(rect.Left + 4, rect.Top + 4, rect.Right - 4, rect.Bottom - 4);
                paint.Color = SKColors.Black;
                canvas.DrawRect(rect, paint);

                rect = new SKRect(rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
                var grad = WeatherBitmapCache.Get("gradient.png");
                canvas.DrawBitmap(grad, rect);
            }

            var w = (rect.Right - rect.Left);

            // day of week
            using (var paint = new SKPaint())
            {
                paint.Color = WeatherColors.Yellow;
                paint.TextSize = 36;
                paint.Typeface = WeatherTypeface.Standard;
                paint.TextAlign = SKTextAlign.Center;

                var forecastTime =
                    new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(forecast.time).ToLocalTime();
                DrawShadowedText(canvas, forecastTime.ToString("ddd").ToUpper(), rect.Left + w/2, rect.Top + 30, paint);
            }

            // forecast icon

            var icon = WeatherBitmapCache.Get($"{forecast.icon}.gif");
            var paddingW = (w - icon.Width)/2;
            var paddingH = (128 - icon.Height)/2;
            var imgRect = new SKRect(rect.Left + paddingW, 140 + paddingH, rect.Right - paddingW,
                140 + 128 - paddingH);
            canvas.DrawBitmap(icon, imgRect);

            // hi/lo
            using (var paint = new SKPaint())
            {
                paint.TextSize = 36;
                paint.Typeface = WeatherTypeface.Standard;
                paint.TextAlign = SKTextAlign.Center;

                paint.Color = WeatherColors.TealAlso;
                DrawShadowedText(canvas, "Lo", rect.Left + w/4, rect.Bottom - 50, paint);

                paint.Color = WeatherColors.Yellow;
                DrawShadowedText(canvas, "Hi", rect.Left + (w/4)*3, rect.Bottom - 50, paint);

                paint.Color = SKColors.White;
                paint.Typeface = WeatherTypeface.Large;
                paint.TextSize = 32;
                DrawShadowedText(canvas, $"{Math.Round(forecast.temperatureMin)}", rect.Left + w/4, rect.Bottom - 5,
                    paint);
                DrawShadowedText(canvas, $"{Math.Round(forecast.temperatureMax)}", rect.Left + (w/4)*3, rect.Bottom - 5,
                    paint);
            }

            // condition
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;
                paint.TextSize = 36;
                paint.Typeface = WeatherTypeface.Standard;
                paint.TextAlign = SKTextAlign.Center;

                var description = forecast.icon.Split('-');

                DrawShadowedText(canvas, TextUtils.UppercaseFirst(description[0]), rect.Left + w/2,
                    rect.Bottom - 110 - 50, paint);
                if (description.Length >= 2)
                {
                    DrawShadowedText(canvas, TextUtils.UppercaseFirst(description[1]), rect.Left + w/2,
                        rect.Bottom - 110,
                        paint);
                }
            }
        }

        private void DrawShadowedText(SKCanvas canvas, string text, float x, float y, SKPaint paint)
        {
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

        public void DrawWeather(WeatherRendererInfo info, SKCanvas canvas, int width, int height)
        {
            // background
            using (var paint = new SKPaint())
            {
                var colors = new[] {WeatherColors.DarkBlue, WeatherColors.Orange};

                var headerHeight = 84;

                var shaderGradient = SKShader.CreateLinearGradient(new SKPoint(0, headerHeight), new SKPoint(0, height),
                    colors, null, SKShaderTileMode.Clamp);
                var shaderNoise = SKShader.CreatePerlinNoiseTurbulence(0.99f, 0.99f, 1, 0.0f);
                var shaderComp = SKShader.CreateCompose(shaderGradient, shaderNoise, SKXferMode.Multiply);

                paint.Color = WeatherColors.DarkBlue;
                paint.Shader = shaderComp;

                // background
                canvas.DrawRect(new SKRect(0, 0, width, height), paint);

                // header background
                var grad = WeatherBitmapCache.Get("gradient_orange.png");
                canvas.DrawBitmap(grad, new SKRect(0, 0, width, headerHeight));

                // top right cutout
                paint.Shader = null;
                using (var path = new SKPath())
                {
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
            using (var paint = new SKPaint())
            {
                var alertsActive = info.WeatherResponse.alerts?.Count > 0;
                var rect = new SKRect(0, height - 100, width, height);

                paint.Color = SKColors.Black;
                canvas.DrawRect(rect, paint);

                rect.Top += 2;
                paint.Color = SKColors.Gray;
                canvas.DrawRect(rect, paint);

                rect.Top += 2;
                paint.Color = alertsActive ? WeatherColors.Red : WeatherColors.Teal;
                canvas.DrawRect(rect, paint);

                paint.Color = SKColors.White;
                paint.TextSize = 36;
                paint.Typeface = WeatherTypeface.Standard;
                string message;
                if (alertsActive)
                {
                    message = string.Join(", ", (from alert in info.WeatherResponse.alerts select alert.title));
                }
                else
                {
                    var unit = info.Unit == Unit.us ? "F" : "C";
                    message =
                        $"Temp: {(int) info.WeatherResponse.currently.temperature}°{unit}   Feels Like: {(int) info.WeatherResponse.currently.apparentTemperature}°{unit}";
                }
                DrawShadowedText(canvas, message, 5, height - 55, paint);
            }

            // logo
            var logo = WeatherBitmapCache.Get("logo.png");
            var imgRect = new SKRect(20, 5, 20 + logo.Width, 5 + logo.Height);
            ;
            canvas.DrawBitmap(logo, imgRect);

            // city name and extended forecast header text
            using (var paint = new SKPaint())
            {
                paint.TextSize = 36.0f;
                paint.Typeface = WeatherTypeface.Standard;
                var baseX = 150.0f;
                var baseY = 32.0f;
                var baseYOff = 30.0f;

                paint.Color = SKColors.White;
                DrawShadowedText(canvas, info.Address, baseX, baseY, paint);

                paint.Color = WeatherColors.Yellow;
                DrawShadowedText(canvas, "Extended Forecast", baseX, baseY + baseYOff, paint);
            }

            // top right date and time
            using (var paint = new SKPaint())
            {
                // TODO: this is probably not right. i have an offset but no clue what the best way is here
                var now = DateTime.UtcNow + TimeSpan.FromHours(info.WeatherResponse.offset);

                var topDateLine = now.ToString("h:mm:ss tt").ToUpper();
                var bottomDateLine = now.ToString("ddd MMM d").ToUpper();

                var baseX = width - 280.0f;
                var baseY = 45;

                paint.Typeface = WeatherTypeface.Small;
                paint.TextSize = 36;
                paint.Color = SKColors.LightGray;

                DrawShadowedText(canvas, topDateLine, baseX, baseY, paint);
                DrawShadowedText(canvas, bottomDateLine, baseX, baseY + 20, paint);
            }

            for (var i = 0; i < 4; i++)
            {
                var boxWidth = 220;
                var boxOffset = width/4;
                DrawForecast(canvas, new SKRect(i*boxOffset + 10, 90, i*boxOffset + boxWidth, height - 90 - 20),
                    info.WeatherResponse.daily.data[i]);
            }
        }
    }
}