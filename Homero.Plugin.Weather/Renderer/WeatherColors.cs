using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Homero.Plugin.Weather.Renderer
{
    public static class WeatherColors
    {
        public static SKColor Blue = new SKColor(8, 15, 255);
        public static SKColor LightBlue = new SKColor(121, 112, 255);
        public static SKColor DarkBlue = new SKColor(41, 25, 92);
        public static SKColor Orange = new SKColor(194, 108, 2);
        public static SKColor Red = new SKColor(198, 19, 2);
        public static SKColor Teal = new SKColor(47, 65, 120);
        // field name copied straight from c:/build/mickr/quakezero/src/engine/colors.h
        public static SKColor TealAlso = new SKColor(172, 177, 237);

        public static SKColor Yellow = new SKColor(205, 185, 0);
    }
}
