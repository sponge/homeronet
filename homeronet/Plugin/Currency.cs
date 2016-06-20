using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Homero.Messages;
using Newtonsoft.Json;

namespace Homero.Plugin
{
    public class Currency : IPlugin
    {
        private WebClient _webClient;
        private const string OUPUT_FORMAT = "{0} {1} is {2} in {3}";
        private const string HELP_MESSAGE = "currency amount currency1 currency2 - convert amount in currency1 to currency2";

        public void Startup()
        {
            _webClient = new WebClient();
        }

        public List<string> RegisteredTextCommands
        {
            get { return new List<string>() { "currency" }; }
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command)
        {
            return new Task<IStandardMessage>(() =>
            {
                if (command.Command == "currency")
                {
                    if (command.Arguments?.Count != 3)
                    {
                        return command.InnerMessage.CreateResponse(HELP_MESSAGE);
                    }
                    decimal amount = Decimal.Parse(command.Arguments.First());
                    string currencyFrom = Uri.EscapeUriString(command.Arguments.ElementAt(1).ToUpper());
                    string currencyTo = Uri.EscapeUriString(command.Arguments.ElementAt(2).ToUpper());
                    string url = String.Format("http://api.fixer.io/latest?symbols={0},{1}&base={0}", currencyFrom, currencyTo);

                    dynamic response = JsonConvert.DeserializeObject<dynamic>(_webClient.DownloadString(url));
                    decimal rate = response["rates"][currencyTo];

                    return command.InnerMessage.CreateResponse(String.Format(OUPUT_FORMAT, amount, currencyFrom, (rate * amount).ToString(), currencyTo));
                }
                return null;
            });
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message)
        {
            return null;
        }

        public void Shutdown()
        {
        }
    }
}
