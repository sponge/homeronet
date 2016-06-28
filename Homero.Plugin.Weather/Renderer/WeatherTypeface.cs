using System.IO;
using Homero.Core.Utility;
using SkiaSharp;

namespace Homero.Plugin.Weather.Renderer
{
    public static class WeatherTypeface
    {
        public static SKTypeface Standard =
            SKTypeface.FromFile(Path.Combine(Paths.ResourceDirectory, "Weather", "Star4000.ttf"));

        public static SKTypeface Large =
            SKTypeface.FromFile(Path.Combine(Paths.ResourceDirectory, "Weather", "Star4000 Large.ttf"));

        public static SKTypeface Small =
            SKTypeface.FromFile(Path.Combine(Paths.ResourceDirectory, "Weather", "Star4000 Small.ttf"));
    }
}