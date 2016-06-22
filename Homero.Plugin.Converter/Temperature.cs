using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin.Converter
{

    public class Temperature : IPlugin
    {
        private List<string> _registeredCommands = new List<string>() { "temperature" };

        public Temperature(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            double temp;
            try
            {
                temp = double.Parse(e.Command.Arguments[0]);
            }
            catch (Exception)
            {
                client?.ReplyTo(e.Command, ".temperature <temp> -- converts <temp> from C to F and F to C");
                return;
            }

            if (Math.Abs(temp) == 420)
            {
                client?.ReplyTo(e.Command, "SMOKE WEED EVERY DAY DONT GIVE A FUCK");
                return;
            }
            else if (Math.Abs(temp) > 500)
            {
                client?.ReplyTo(e.Command, "2 hot 4 u");
                return;
            }

            double c = (temp - 32) * (5.0 / 9.0);
            double f = (temp * (9.0 / 5.0)) + 32;

            client?.ReplyTo(e.Command, String.Format("{0:0.0}F is {1:0.0}C. {0:0.0}C is {2:0.0}F.", temp, c, f));
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }
    }
}