using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Examples
{
    public class ExamplePlugin : IPlugin
    {
        [Command]
        public string Hello()
        {
            return "Hey friend.";
        }

        [Command]
        public void DelayHello(IInvocationContext context, IMessage message)
        {
        }

        [Command]
        public IMessage EchoHello(IInvocationContext context, IMessage message)
        {
            return new Message(message);
        }

        public void Dispose()
        {
        }
    }
}