using System;
using System.Collections.Generic;
using System.IO;
using Homero.Core.Utility;
using Newtonsoft.Json;

namespace Homero.Core.Services
{
    public interface IConfigurationFactory
    {
        IConfiguration GetConfiguration(string ClassType);
    }

    public class JsonConfigurationFactory : IConfigurationFactory
    {
        private static string CONFIG_FILE = Path.Combine(Paths.DataDirectory, "config.json");

        private Dictionary<string, IConfiguration> _backedConfigurations;

        private Dictionary<string, object> _rootConfig;

        public JsonConfigurationFactory()
        {
            if (_instance != null)
            {
                // Ensure only one instance of a configuration is grabbed from the factory.
                throw new Exception("DOUBLE FACTORY OH NO");
            }
            _backedConfigurations = new Dictionary<string, IConfiguration>();

            if (File.Exists(CONFIG_FILE))
            {
                try
                {
                    _rootConfig = Json.CleanDictionary(JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(CONFIG_FILE)));
                    if (_rootConfig == null)
                    {
                        throw new Exception("Null dict.");
                    }

                }
                catch (Exception)
                {
                    _rootConfig = new Dictionary<string, object>();
                }
            }
            else
            {
                File.Create(CONFIG_FILE);
                _rootConfig = new Dictionary<string, object>();
            }
        }

        public IConfiguration GetConfiguration(string ClassType)
        {
            if (ClassType == null)
            {
                ClassType = "Core";
            }

            if (_backedConfigurations.ContainsKey(ClassType))
            {
                return _backedConfigurations[ClassType];
            }

            IConfiguration config = new Configuration(ClassType);
            _backedConfigurations.Add(ClassType, config);
            if (_rootConfig.ContainsKey(ClassType))
            {
                config.Update((Dictionary<string, object>) _rootConfig[ClassType]);
            }
            config.Changed += ConfigOnChanged;
            return config;
        }

        private void ConfigOnChanged(object sender, System.EventArgs eventArgs)
        {
            IConfiguration configObj = sender as IConfiguration;
            if (configObj != null)
            {
                _rootConfig[configObj.Name] = configObj.BackingDictionary;
            }
            File.WriteAllText(CONFIG_FILE, JsonConvert.SerializeObject(_rootConfig, Formatting.Indented));
        }

        #region Singleton

        private static JsonConfigurationFactory _instance;

        public static JsonConfigurationFactory Instance
        {
            get { return _instance ?? (_instance = new JsonConfigurationFactory()); }
        }

        #endregion
    }
}