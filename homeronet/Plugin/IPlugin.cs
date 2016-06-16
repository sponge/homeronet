using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using homeronet.Messages;

namespace homeronet.Plugins
{
    public interface IPlugin
    {
        void Startup();
        void Shutdown();

        Task<IStandardMessage> HandleTextCommandInvocationAsync(ITextCommand command);
        List<string> RegisteredTextCommands { get; }
        Task<IStandardMessage> ProcessTextMessage(IStandardMessage message);
    }
}
