using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.Messages;
using System.Net;
using Newtonsoft.Json;

namespace homeronet.Plugin
{
    public class Currency : IPlugin
    {
        private WebClient _webClient;
        private string _outputFormat = "{0} {1} is {2} in {3}";
        private string _helpMessage = "currency amount currency1 currency2 - convert amount in currency1 to currency2";

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
                    if (command.Arguments.Count != 3)
                    {
                        return command.InnerMessage.CreateResponse(_helpMessage);
                    }
                    decimal amount = Decimal.Parse(command.Arguments.First());
                    string currencyFrom = Uri.EscapeUriString(command.Arguments.ElementAt(1).ToUpper());
                    string currencyTo = Uri.EscapeUriString(command.Arguments.ElementAt(2).ToUpper());
                    string url = String.Format("http://api.fixer.io/latest?symbols={0},{1}&base={0}", currencyFrom, currencyTo);

                    dynamic response = JsonConvert.DeserializeObject<dynamic>(_webClient.DownloadString(url));
                    decimal rate = response["rates"][currencyTo];

                    return command.InnerMessage.CreateResponse(String.Format(_outputFormat, amount, currencyFrom, (rate * amount).ToString(), currencyTo));
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
