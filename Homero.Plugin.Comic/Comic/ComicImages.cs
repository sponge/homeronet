using Homero.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Comic
{
    public static class ComicImages
    {
        public static List<string> Charcters = Directory.GetFiles(Path.Combine(Paths.ResourceDirectory, "Comic/Characters")).ToList();
        public static List<string> Backgrounds = Directory.GetFiles(Path.Combine(Paths.ResourceDirectory, "Comic/Backgrounds")).ToList();
    }
}
