using System.Collections.Generic;

namespace Homero.Core
{
    public interface IPlugin
    {
        List<string> RegisteredTextCommands { get; }

        void Startup();

        void Shutdown();
    }
}