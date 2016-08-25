using Homero.Core.Client;
using Homero.Core.Messages;
using Ninject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homero.Plugin
{
    public class IrcReflect : IPlugin
    {
        private IKernel _boundKernel;

        public IrcReflect(IKernel kernel)
        {
            _boundKernel = kernel;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; }
    }
}