using Homero.Core;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Homero.Plugin.Converter
{
    public class Currency : IPlugin
    {
        private const string OUPUT_FORMAT = "{0} {1} is {2} in {3}";

        private const string HELP_MESSAGE =
            "currency amount currency1 currency2 - convert amount in currency1 to currency2";

        private WebClient _webClient;

        public Currency(IMessageBroker broker)
        {
            _webClient = new WebClient();
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public List<string> RegisteredTextCommands
        {
            get { return new List<string> { "currency" }; }
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            if (e.Command.Arguments?.Count != 3)
            {
                e.ReplyTarget.Send(HELP_MESSAGE);
                return;
            }
            var amount = decimal.Parse(e.Command.Arguments.First());
            var currencyFrom = Uri.EscapeUriString(e.Command.Arguments.ElementAt(1).ToUpper());
            var currencyTo = Uri.EscapeUriString(e.Command.Arguments.ElementAt(2).ToUpper());
            var url = string.Format("http://api.fixer.io/latest?symbols={0},{1}&base={0}", currencyFrom, currencyTo);

            dynamic response = JsonConvert.DeserializeObject<dynamic>(_webClient.DownloadString(url));
            decimal rate = response["rates"][currencyTo];

            e.ReplyTarget.Send(string.Format(OUPUT_FORMAT, amount, currencyFrom, rate * amount, currencyTo));
        }
    }
}