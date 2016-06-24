using Homero.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.EventArgs;
using Homero.Client;

namespace Homero.Plugin.Goon
{
    public class DominionsRandomNumber : IPlugin
    {
        /*
        Most Dominions game mechanisms use something called the Dominions Random Number (DRN).
        When a random number is called for, the number used is actually a DRN. This is a roll of two six-sided
        dice (2d6) but with an additional bonus: if any individual die roll is “6,” one is subtracted, and then that
        die is re-rolled and added to the result
        */
        private Random _random;

        public List<string> RegisteredTextCommands
        {
            get { return new List<string> { "drn" }; }
        }

        public DominionsRandomNumber(IMessageBroker broker)
        {
            _random = new Random();
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            int drn = RollD6() + RollD6();
            client.ReplyTo(e.Command, drn.ToString());
        }

        private int RollD6()
        {
            int roll = _random.Next(6) + 1;

            if (roll == 6)
            {
                // explode!!
                return 5 + RollD6();
            }

            return roll;
        }

        public void Shutdown()
        {
        }

        public void Startup()
        {
        }
    }
}
