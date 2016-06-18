using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Messages;
using homeronet.Plugin;
using homeronet.Services;

namespace Homeronet.Plugin.Standard
{
    public class Ping : IPlugin
    {
        private readonly IConfiguration _config;
        public Ping(IConfiguration config)
        {
            _config = config;
        }

        public void Startup()
        {

        }

        public void Shutdown()
        {
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
