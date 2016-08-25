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
        private List<ComicMessage> _messages;
        
        public CharacterMappings Mappings { get; set; }
        public string Background { get; set; }
        public List<ComicPanel> Panels { get; set; }

        public Comic(List<ComicMessage> messages)
        {
            Random random = new Random();

            _messages = messages;
            Mappings = new CharacterMappings();
            Panels = new List<ComicPanel>();
            Background = ComicImages.Backgrounds[random.Next(ComicImages.Backgrounds.Count)];

            Regenerate();
        }

        public Comic(List<ComicMessage> messages, string title)
            : this(messages)
        {
            Panels.Insert(0, new ComicPanel
            {
                IsTitle = true,
                Messages = new List<ComicMessage>
                {
                    new ComicMessage { Message = title },
                }
            });
        }

        public Comic(Comic prevComic)
        {
            _messages = prevComic._messages;
            Mappings = prevComic.Mappings;
            Background = prevComic.Background;
            Panels = prevComic.Panels;
        }

        public void Regenerate()
        {
            ComicPanel currentPanel = new ComicPanel();

            foreach (ComicMessage message in _messages)
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
