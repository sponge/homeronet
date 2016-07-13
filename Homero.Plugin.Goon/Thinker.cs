using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homero.Plugin.Goon
{
    public class Thinker : IPlugin
    {
        private const string ThinkerTemplate = "( .   __ . ) . o O ( {0} )\n";
        private const string ThinkerEmptyLnTemplate = "                     {0}\n";
        private ILogger _logger;

        public Thinker(IMessageBroker broker, ILogger logger)
        {
            _logger = logger;
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
            _logger.Debug("( .__. ).o O( thinkin real hard about startin' up )");
        }

        public void Shutdown()
        {
            _logger.Debug("( .__. ).o O( thinkin real hard about going away now... )");
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "think" };

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;
            var thought = string.Join(" ", e.Command.Arguments).Split('\n');
            var thinkerLine = (int)Math.Ceiling(thought.Length / 2.0f);

            var response = new StringBuilder();

            if (client?.MarkdownSupported == true)
            {
                response.Append("```");
            }

            for (var i = 0; i < thought.Length; i++)
            {
                var lnUsed = i + 1 == thinkerLine ? ThinkerTemplate : ThinkerEmptyLnTemplate;
                response.AppendFormat(lnUsed, thought[i]);
            }

            if (client?.MarkdownSupported == true)
            {
                response.Append("```");
            }

            e.ReplyTarget.Send(response.ToString());
        }
    }
}