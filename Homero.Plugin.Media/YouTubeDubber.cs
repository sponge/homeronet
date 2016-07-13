using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Messages;
using Homero.Core.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homero.Plugin.Media
{
    public class YouTubeDubber : IPlugin
    {
        private Dictionary<string, string> _builtins = new Dictionary<string, string>
        {
            {"whale", "http://www.youtube.com/watch?v=ZS_6-IwMPjM"},
            {"cow", "http://www.youtube.com/watch?v=lXKDu6cdXLI"},
            {"lawnmower", "http://www.youtube.com/watch?v=r6FpEjY1fg8"},
            {"worldstar", "https://www.youtube.com/watch?v=uEgtNSBa4Zk"}
        };

        private Regex _ytRegex;

        public YouTubeDubber(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
            _ytRegex = new Regex("(?:http|https)://(?:www\\.)?(?:youtube\\.com/watch\\?v=|youtu\\.be/)([^&\n]+)",
                RegexOptions.Compiled);
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string>
        {
            "lawnmower",
            "whale",
            "cow",
            "worldstar",
            "dub"
        };

        public string Dub(string video, string audio, int delay)
        {
            var videoMatch = _ytRegex.Match(video);
            var audioMatch = _ytRegex.Match(audio);

            if (videoMatch.Groups.Count != 2 && audioMatch.Groups.Count != 2)
            {
                return "couldn't parse that one m8";
            }

            video = videoMatch.Groups[1].Value;
            audio = audioMatch.Groups[1].Value;

            return $"http://www.youdubber.com/index.php?video={video}&audio={audio}&audio_start={delay}";
        }

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var delay = 0;

            var minArgCount = e.Command.Command == "dub" ? 2 : 1;

            if (e.Command.Arguments.Count < minArgCount)
            {
                e.ReplyTarget.Send(".dub < vid > < audio > [audio start time]-- tubedubber");
                return;
            }

            if (e.Command.Arguments.Count > minArgCount)
            {
                var success = int.TryParse(e.Command.Arguments[minArgCount], out delay);
                if (!success)
                {
                    e.ReplyTarget.Send("that is not a time");
                    return;
                }
            }

            string video = null, audio = null;
            if (e.Command.Command == "dub")
            {
                video = e.Command.Arguments[0];
                audio = e.Command.Arguments[1];
            }
            else if (e.Command.Command == "worldstar")
            {
                video = e.Command.Arguments[0];
                audio = _builtins[e.Command.Command];
            }
            else
            {
                video = _builtins[e.Command.Command];
                audio = e.Command.Arguments[0];
            }

            var strOut = Dub(video, audio, delay);

            e.ReplyTarget.Send(strOut);
        }
    }
}