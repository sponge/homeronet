using Homero.Core.EventArgs;
using Homero.Core.Services;
using Homero.Core.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Homero.Plugin.Converter
{
    internal class BitcoinPricePoint
    {
        public float Low { get; set; }
        public float Average { get; set; }
        public float High { get; set; }
    }
    public class Bitcoin : IPlugin
    {
        private List<BitcoinPricePoint> _btcHistory = new List<BitcoinPricePoint>();
        private static float CICIS_CHEESE_PIZZA_COST = 6.99f;

        private List<string> _registeredCommands = new List<string>() { "bitcoin" };
        public Bitcoin(IMessageBroker broker)
        {
            broker.CommandReceived += Broker_CommandReceived;
            DispatcherTimer btcUpdater = new DispatcherTimer();
            btcUpdater.Tick += BtcUpdater_Tick;
            btcUpdater.Interval = TimeSpan.FromMinutes(5);
            btcUpdater.Start();
            BtcUpdater_Tick(this, null);
        }

        private async void BtcUpdater_Tick(object sender, EventArgs e)
        {
            if(_btcHistory.Count >= 10)
            {
                _btcHistory.RemoveAt(0);
            }

            JObject currentPrice = await Web.GetJsonAsync("https://www.bitstamp.net/api/ticker/");
            if(currentPrice != null)
            {
                BitcoinPricePoint btcPoint = new BitcoinPricePoint()
                {
                    Low = (float) currentPrice["low"],
                    Average = (float) currentPrice["last"],
                    High = (float) currentPrice["high"]
                };

                _btcHistory.Add(btcPoint);
            }
        }

        public List<string> RegisteredTextCommands
        {
            get
            {
                return _registeredCommands;
            }
        }

        public void Shutdown()
        {
        }

        public void Startup()
        {
        }

        private void Broker_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            if(_btcHistory.Count == 0)
            {
                e.ReplyTarget.Send("i have no bitcoin data because it's bad");
                return;
            }

            int cicisPizzas = Convert.ToInt32(Math.Floor(_btcHistory.Last().Average / CICIS_CHEESE_PIZZA_COST));
            e.ReplyTarget.Send($"Current: ${_btcHistory.Last().Average} | High: ${_btcHistory.Last().High} | Low: ${_btcHistory.Last().Low} | Cici's 🍕 worth: {cicisPizzas} | Graph: {CreateSparklineFromValues(_btcHistory.Select(x => x.Average).ToList())}");
        }

        private string CreateSparklineFromValues(List<float> values)
        {
            double max = values.Max();
            double min = values.Min();
            double rn = max - min;
            List<char> characterBuffer = new List<char>();

            foreach(double value in values)
            {
                int sparkValue = 0;

                if(value - min != 0 && rn != 0)
                {
                    sparkValue = Convert.ToInt32(Math.Floor(Math.Min(6, ((value - min) / rn) * 7)));
                }
                characterBuffer.Add(Convert.ToChar(9601 + sparkValue));
            }
            return string.Concat(characterBuffer);
        }
    }
}
