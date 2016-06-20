using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Messages;
using System.Net;
using Newtonsoft.Json;
using homeronet.Services;
using homeronet.EventArgs;
using homeronet.Client;

namespace homeronet.Plugin
{
    public class Currency : IPlugin
    {
        private WebClient _webClient;
        private const string OUPUT_FORMAT = "{0} {1} is {2} in {3}";
        private const string HELP_MESSAGE = "currency amount currency1 currency2 - convert amount in currency1 to currency2";

        public Currency(IMessageBroker broker)
        {
            _webClient = new WebClient();
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public List<string> RegisteredTextCommands
        {
            get { return new List<string>() { "currency" }; }
        }

        public void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;

            if (e.Command.Arguments?.Count != 3)
            {
                client.ReplyTo(e.Command, HELP_MESSAGE);
                return;
            }
            decimal amount = Decimal.Parse(e.Command.Arguments.First());
            string currencyFrom = Uri.EscapeUriString(e.Command.Arguments.ElementAt(1).ToUpper());
            string currencyTo = Uri.EscapeUriString(e.Command.Arguments.ElementAt(2).ToUpper());
            string url = String.Format("http://api.fixer.io/latest?symbols={0},{1}&base={0}", currencyFrom, currencyTo);

            dynamic response = JsonConvert.DeserializeObject<dynamic>(_webClient.DownloadString(url));
            decimal rate = response["rates"][currencyTo];

            client.ReplyTo(e.Command, String.Format(OUPUT_FORMAT, amount, currencyFrom, (rate * amount).ToString(), currencyTo));
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }
    }
}
