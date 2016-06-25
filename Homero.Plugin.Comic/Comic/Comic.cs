using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Comic
{
    public class Comic
    {
        public string Title { get; set; }
        public CharacterMappings Mappings { get; set; }
        public string Background { get; set; }
        public List<ComicPanel> Panels { get; set; }

        public Comic(List<ComicMessage> messages)
        {
            Mappings = new CharacterMappings();
            Panels = new List<ComicPanel>();
            ComicPanel currentPanel = new ComicPanel();

            Random random = new Random();
            Background = ComicImages.Backgrounds[random.Next(ComicImages.Backgrounds.Count)];

            foreach (ComicMessage message in messages)
            {
                if (currentPanel.Messages.Count == 2 || (currentPanel.Messages.Count == 1 && message.User == currentPanel.Messages.ElementAt(0).User))
                {
                    Panels.Add(currentPanel);
                    currentPanel = new ComicPanel();
                }

                Mappings.Add(message.User);
                currentPanel.Messages.Add(message);
            }

            if (currentPanel.Messages.Count > 0)
            {
                Panels.Add(currentPanel);
            }
        }
    }
}
