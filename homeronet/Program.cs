using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using homeronet.Client;
using homeronet.Plugins;
using Newtonsoft.Json;
using Ninject;
using Ninject.Parameters;
using NLog;

namespace homeronet {

    public class Program
    {
        public static IKernel Kernel { get; private set; }
        public static Logger Log { get; private set; }
        private static void Main(string[] args)
        {
            Log = LogManager.GetCurrentClassLogger();
            
            Log.Info("Homero.NET - Startup");

            Log.Debug("Building kernel");
            Kernel = new StandardKernel(new HomeroModule());

            Log.Debug("Building configuration factory");
            Kernel.Bind<IClientConfiguration>().ToMethod((context =>
            {
                // TODO: Proper config factory.
                IParameter clientNameParam = context.Parameters.FirstOrDefault(x => x.Name == "ClientName");
                if (clientNameParam != null)
                {
                    string clientName = clientNameParam.GetValue(context, null) as string;
                    using (StreamReader r = new StreamReader("config.json"))
                    {
                        // TODO: Strict contract
                        string json = r.ReadToEnd();
                        dynamic jsonConfig = JsonConvert.DeserializeObject<dynamic>(json);
                        return new ClientConfiguration() { ApiKey = jsonConfig[clientName]["ApiKey"].ToString(), Username = jsonConfig[clientName]["Username"].ToString(), Password = jsonConfig[clientName]["Password"].ToString() };
                    }
                }
                return null;
            }));

            Log.Debug("Binding Discord");
            Kernel.Bind<IClient>().To<DiscordClient>().InSingletonScope().WithParameter(new Parameter("ClientName", "DiscordClient", true));

            foreach (IClient client in Kernel.GetAll<IClient>())
            {
                Log.Info($"Connecting {client.GetType()}.");
                try
                {
                    client.Connect();
                    Log.Info($"{client.GetType()} connected.");
                }
                catch (Exception e)
                {
                    Log.Warn($"Error connecting {client.GetType()}.");
                    Log.Error(e);
                }
            }

            Log.Info("Homero running. Press any key to quit.");
            Console.ReadKey();
        }
    }
}