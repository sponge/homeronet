using System.Collections.Generic;

namespace Homero.Plugin
{
    public interface IPlugin
    {
        void Startup();
        void Shutdown();
        List<string> RegisteredTextCommands { get; }
    }
}
