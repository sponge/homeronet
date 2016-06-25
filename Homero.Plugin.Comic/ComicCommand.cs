using Homero.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.EventArgs;
using Homero.Client;
using System.IO;
using System.Linq;
using SkiaSharp;
using Homero.Messages.Attachments;

namespace Homero.Plugin.Comic
{
    public class ComicCommand : IPlugin
    {
        private const int QUEUE_LIMIT = 30;
        private Dictionary<string, MessageQueue<ComicMessage>> _queues;

        public List<string> RegisteredTextCommands
        {
            get { return new List<string> { "comic" }; }
        }

        public ComicCommand(IMessageBroker broker)
        {
            _queues = new Dictionary<string, MessageQueue<ComicMessage>>();
            broker.CommandReceived += BrokerOnCommandReceived;
            broker.MessageReceived += BrokerOnMessageReceived;
        }

        private void BrokerOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (!_queues.ContainsKey(e.Message.Channel))
            {
                _queues.Add(e.Message.Channel, new MessageQueue<ComicMessage>(QUEUE_LIMIT));
            }
            _queues[e.Message.Channel].Add(new ComicMessage()
            {
                Message = e.Message.Message,
                User = e.Message.Sender,
            });
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;

            Comic comic = new Comic(_queues[e.Command.InnerMessage.Channel].ToList());
            Stream stream = CreateComic(comic);

            client.ReplyTo(e.Command, new ImageAttachment() { DataStream = stream, Name = $"{e.Command.InnerMessage.Sender} Comic {DateTime.Now}.png" });
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
