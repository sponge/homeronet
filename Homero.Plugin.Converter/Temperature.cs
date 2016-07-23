using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System;
using System.Collections.Generic;
using Homero.Core.Interface;

namespace Homero.Plugin.Converter
{
    public class Temperature : IPlugin
    {
        public Temperature(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "temperature" };

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;
            double temp;
            try
            {
                temp = double.Parse(e.Command.Arguments[0]);
            }
            catch (Exception)
            {
                e.ReplyTarget.Send(".temperature <temp> -- converts <temp> from C to F and F to C");
                return;
            }

            if (Math.Abs(temp) == 420)
            {
                e.ReplyTarget.Send("SMOKE WEED EVERY DAY DONT GIVE A FUCK");
                return;
            }
            if (Math.Abs(temp) > 500)
            {
                e.ReplyTarget.Send("2 hot 4 u");
                return;
            }

            var c = (temp - 32) * (5.0 / 9.0);
            var f = (temp * (9.0 / 5.0)) + 32;

            e.ReplyTarget.Send(string.Format("{0:0.0}F is {1:0.0}C. {0:0.0}C is {2:0.0}F.", temp, c, f));
        }
    }
}