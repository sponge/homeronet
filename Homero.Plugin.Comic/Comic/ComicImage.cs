using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Homero.Plugin.Comic
{
    public class ComicImage
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string Filename { get; set; }

        public ComicImage(string name, string filename)
        {
            Name = name;
            Filename = filename;
        }
    }
}
