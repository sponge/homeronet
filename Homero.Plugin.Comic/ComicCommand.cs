using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SkiaSharp;
using Homero.Core.Services;
using Homero.Core.EventArgs;
using Homero.Core.Interface;
using Homero.Core.Messages.Attachments;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Homero.Plugin.Comic
{
    public class ComicCommand : IPlugin
    {
        private const int QUEUE_LIMIT = 30;
        private Dictionary<string, MessageQueue<ComicMessage>> _queues;
        private Comic _lastComic;

        public List<string> RegisteredTextCommands
        {
            get { return new List<string> { "comic", "lastcomic", "importcomic" }; }
        }

        public ComicCommand(IMessageBroker broker)
        {
            _queues = new Dictionary<string, MessageQueue<ComicMessage>>();
            broker.CommandReceived += BrokerOnCommandReceived;
            broker.MessageReceived += BrokerOnMessageReceived;
        }

        private void BrokerOnMessageReceived(object sender, MessageEventArgs e)
        {
            if (!_queues.ContainsKey(e.Channel.Name))
            {
                _queues.Add(e.Channel.Name, new MessageQueue<ComicMessage>(QUEUE_LIMIT));
            }
            _queues[e.Channel.Name].Add(new ComicMessage()
            {
                Message = e.Message.Message,
                User = e.User.Name,
            });
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;

            if (e.Command.Command == "comic")
            {
                HandleComic(client, e);
            }
            else if (e.Command.Command == "lastcomic")
            {
                HandleLastComic(client, e);
            }
            else if (e.Command.Command == "importcomic")
            {
                //HandleImportComic(client, e);
            }
        }

        private void HandleLastComic(IClient client, CommandReceivedEventArgs e)
        {
            /*
            .lastcomic [action] [1,2,3...]

            actions:
                none    repost last comic
                retry   regen last comic with random bg/characters
                export  export JSON repr of last comic
            */
            Regex test = new Regex(@"^(retry|export)?\s?((?:\d+)(?:,\s*\d+)*)?$");
            Match match = test.Match(String.Join(" ", e.Command.Arguments));

            if (_lastComic == null)
            {
                e.ReplyTarget.Send("no last comic");
                return;
            }

            if (!match.Success)
            {
                e.ReplyTarget.Send("invalid arguments");
                return;
            }

            string command = "";
            int panelIndex = 1;
            List<int> panels;
            if (match.Groups.Count == 3)
            {
                command = match.Groups[1].Value;
                panelIndex = 2;
            }

            if (!String.IsNullOrEmpty(match.Groups[panelIndex].Value))
            {
                panels = match.Groups[panelIndex].Value.Split(',').Select(Int32.Parse).ToList();
                Stream stream = CreateSlicedComic(_lastComic, panels);
                e.ReplyTarget.Send("comic", new ImageAttachment() { DataStream = stream, Name = $"{e.ReplyTarget.Name} Comic {DateTime.Now}.png" });
            }

            if (command == "retry")
            {
                _lastComic.Regenerate();
                Stream stream = CreateComic(_lastComic);
                e.ReplyTarget.Send("comic", new ImageAttachment() { DataStream = stream, Name = $"{e.ReplyTarget.Name} Comic {DateTime.Now}.png" });
            }
            else if (command == "export")
            {
                string serialized = JsonConvert.SerializeObject(_lastComic);
                e.ReplyTarget.Send("```" + serialized+ "```");
                return;
            }
        }

        private void HandleComic(IClient sender, CommandReceivedEventArgs e)
        {
            Comic comic;
            if (e.Command.Arguments.Count == 0)
            {
                comic = new Comic(_queues[e.Channel.Name].ToList());
            }
            else
            {
                comic = new Comic(_queues[e.Channel.Name].ToList(), String.Join(" ", e.Command.Arguments.ToArray()));
            }

            Stream stream = CreateComic(comic);
            _lastComic = comic;

            e.ReplyTarget.Send("comic", new ImageAttachment() { DataStream = stream, Name = $"{e.ReplyTarget.Name} Comic {DateTime.Now}.png" });
        }

        private void HandleImportComic(IClient client, CommandReceivedEventArgs e)
        {
            string raw = String.Join(" ", e.Command.Arguments.ToArray());
            Comic imported = JsonConvert.DeserializeObject<Comic>(raw);
            Stream stream = CreateComic(imported);
            e.ReplyTarget.Send("comic", new ImageAttachment() { DataStream = stream, Name = $"{e.ReplyTarget.Name} Comic {DateTime.Now}.png" });
        }

        private Stream CreateSlicedComic(Comic comic, List<int> panels)
        {
            Comic slicedComic = new Comic(comic);
            slicedComic.Panels = panels.Select(i => slicedComic.Panels.Count > i-1 ? slicedComic.Panels[i-1] : null).ToList();
            return CreateComic(slicedComic);
        }

        private Stream CreateComic(Comic comic)
        {
            ComicRenderer comicRenderer = new ComicRenderer();
            int width = 450, height = 300 * comic.Panels.Count;

            using (var surface = SKSurface.Create(width, height, SKColorType.N_32, SKAlphaType.Opaque))
            {
                var skcanvas = surface.Canvas;
                comicRenderer.DrawComic(comic, skcanvas, width, height);
                var img = surface.Snapshot().Encode();
                return img.AsStream();
            }
        }

        public void Shutdown()
        {
        }

        public void Startup()
        {
        }
    }
}
