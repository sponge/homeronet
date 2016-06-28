using System.Collections.Generic;
using System.IO;
using Homero.Core.Utility;
using SkiaSharp;

namespace Homero.Plugin.Weather.Renderer
{
    /// <summary>
    /// Resource bitmap in-memory caching mechanism.
    /// </summary>
    public static class WeatherBitmapCache
    {
        private static Dictionary<string, SKBitmap> _backingCache = new Dictionary<string, SKBitmap>();

        public static SKBitmap Get(string fileName)
        {
            fileName = Path.Combine(Paths.ResourceDirectory, "Weather", fileName);
            if (!File.Exists(fileName))
            {
                return null;
            }

            if (_backingCache.ContainsKey(fileName))
            {
                return _backingCache[fileName];
            }

            _backingCache[fileName] = SKBitmap.Decode(fileName);
            return _backingCache[fileName];
        }
    }
}