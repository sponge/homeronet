using System.Collections.Generic;

namespace Homero.Plugin
{
    public interface IPlugin
    {
        List<string> RegisteredTextCommands { get; }

        void Startup();

        void Shutdown();
    }
}