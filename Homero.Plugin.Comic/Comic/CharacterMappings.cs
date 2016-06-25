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
        private Dictionary<string, string> _mappings;
        private List<string> _characterFiles;
        private int index = 0;
        private Random _random;

        public CharacterMappings()
        {
            _mappings = new Dictionary<string, string>();
            _random = new Random();

            _characterFiles = ComicImages.Charcters.ToList();
            _characterFiles.Sort((a, b) => _random.NextDouble() > 0.5f ? -1 : 1);
        }

        public string Add(string user)
        {
            if (!_mappings.ContainsKey(user))
            {
                string file = GetCharacterFile();
                _mappings.Add(user, file);
            }
            return Get(user);
        }

        public string Get(string user)
        {
            string value;
            _mappings.TryGetValue(user, out value);
            return value;
        }

        private string GetCharacterFile()
        {
            index = (index + 1) % _characterFiles.Count;
            return _characterFiles.ElementAt(index);
        }
    }
}
