using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Messages;
using Homero.Plugin;
using Homero.Services;

namespace Homeronet.Plugin.Standard
{
    public class Ping : IPlugin
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public Ping(IConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Startup()
        {
            _logger.Info("I ping 2!");
        }

        public void Shutdown()
        {
            _logger.Info("I no longer ping!");
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command)
        {
            return null;
        }

        public List<string> RegisteredTextCommands { get; }
        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }
    }
}
