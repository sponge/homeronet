using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Comic
{
    public class CharacterMappings
    {
        public Dictionary<string, string> Mappings;
        private List<string> _characterFiles;
        private int index = 0;
        private Random _random;

        public CharacterMappings()
        {
            Mappings = new Dictionary<string, string>();
            _random = new Random();

            _characterFiles = ComicImages.Charcters.ToList();
            _characterFiles.Sort((a, b) => _random.NextDouble() > 0.5f ? -1 : 1);
        }

        public string Add(string user)
        {
            if (!Mappings.ContainsKey(user))
            {
                string file = GetCharacterFile();
                Mappings.Add(user, file);
            }
            return Get(user);
        }

        public string Get(string user)
        {
            string value;
            Mappings.TryGetValue(user, out value);
            return value;
        }

        private string GetCharacterFile()
        {
            index = (index + 1) % _characterFiles.Count;
            return _characterFiles.ElementAt(index);
        }
    }
}
