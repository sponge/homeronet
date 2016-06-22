using System;
using System.Collections.Generic;
using System.Text;
using Homero.Client;
using Homero.EventArgs;
using Homero.Services;

namespace Homero.Plugin.Goon
{
    public class Thinker : IPlugin
    {
        private List<string> _registeredTextCommands = new List<string>() { "think" };
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

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            string[] thought = string.Join(" ", e.Command.Arguments).Split('\n');
            int thinkerLine = (int) Math.Ceiling(thought.Length / 2.0f);

            StringBuilder response = new StringBuilder();

            if (client?.MarkdownSupported == true)
            {
                response.Append("```");
            }

            for (var i = 0; i < thought.Length; i++)
            {
                string lnUsed = i + 1 == thinkerLine ? ThinkerTemplate : ThinkerEmptyLnTemplate;
                response.AppendFormat(lnUsed, thought[i]);
            }

            if (client?.MarkdownSupported == true)
            {
                response.Append("```");
            }


            client?.ReplyTo(e.Command, response.ToString());
        }


        public List<string> RegisteredTextCommands
        {
            get { return _registeredTextCommands; }
        }
    }
}
