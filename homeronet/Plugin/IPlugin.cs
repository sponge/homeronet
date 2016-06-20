using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using homeronet.Messages;

namespace homeronet.Plugin
{
    public interface IPlugin
    {
        void Startup();
        void Shutdown();
        // List<string> RegisteredTextCommands { get; }
    }
}
