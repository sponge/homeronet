using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Comic
{
    public class ComicCharacter
    {
        public string Name { get; set; }
        public ComicImage CharacterImage { get; set; }

        public ComicCharacter()
        {
            // For serialized characters.
        }

        /// <summary>
        /// Validates if the character has the assets required and updates anything incorrect.
        /// </summary>
        /// <param name="verficiationList">A list of character ComicImage's.</param>
        /// <returns></returns>
        public bool Verify(List<ComicImage> verficiationList)
    }
}
