using ForecastIO;
using GeocodeSharp.Google;
using homeronet.Client;
using homeronet.Messages;
using Ninject;
using Ninject.Parameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace homeronet.Plugin {

    public class Weather : IPlugin {
        private GeocodeClient _geocode;
        private IClientConfiguration forecastConfig;

        private List<string> _registeredCommands = new List<string>()
        {
            "weather"
        };

        public void Startup() {
            forecastConfig = Program.Kernel.Get<IClientConfiguration>(new Parameter("ClientName", "Forecast.IO", true));

            var geocodeConfig = Program.Kernel.Get<IClientConfiguration>(new Parameter("ClientName", "GoogleGeocode", true));
            if (geocodeConfig.ApiKey.Length > 0) {
                _geocode = new GeocodeClient(geocodeConfig.ApiKey);
            }
            else {
                _geocode = new GeocodeClient();
            }

            // TODO: initialize persistent store for username -> location mapping
        }

        public void Shutdown() {
        }

        public Task<IStandardMessage> ProcessTextCommand(ITextCommand command) {
            return new Task<IStandardMessage>(() => {
                string inputLocation = null;
                bool noSave = false, locationValid = false;
                float lat = 0.0f, lng = 0.0f;
                IClient sendingClient = command.InnerMessage.SendingClient;

                // parse out commandline
                if (command.Arguments.Count > 0) {
                    inputLocation = command.Arguments[0];
                }

                if (command.Arguments.Count > 1) {
                    noSave = command.Arguments[1] == "nosave";
                }

                if (inputLocation == null) {
                    // TODO: lookup location based on username, set locationValid to true if we found one
                }

                if (!locationValid && inputLocation != null) {
                    // TODO: migrate to async
                    var geo = _geocode.GeocodeAddress(inputLocation).Result;
                    if (geo.Status == GeocodeStatus.Ok) {
                        var firstResult = geo.Results[0];
                        var location = firstResult.Geometry.Location;
                        lat = (float)location.Latitude;
                        lng = (float)location.Longitude;
                        locationValid = true;
                    }
                }

                if (!locationValid) {
                    return command.InnerMessage.CreateResponse("gotta give me a zipcode or something");
                }

                var weather = new ForecastIORequest(forecastConfig.ApiKey, lat, lng, Unit.si).Get();

                // TODO: create string

                // TODO: create forecast based on discord or irc

                // TODO: save to persistent store for username if dontsave isn't specified

                sendingClient.SendMessage(command.InnerMessage.CreateResponse("weather goes here"));

                return null;
            });
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public Task<IStandardMessage> ProcessTextMessage(IStandardMessage message) {
            return null;
        }
    }
}