using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Services
{
    public interface IConfigurationFactory
    {
        IConfiguration GetConfiguration(string ClassType);
    }

    public class ConfigurationFactory : IConfigurationFactory
    {

        #region Singleton
        private static ConfigurationFactory _instance;
        public static ConfigurationFactory Instance
        {
            get { return _instance ?? (_instance = new ConfigurationFactory()); }
        }

        #endregion

        private Dictionary<string, IConfiguration> _backedConfigurations; // Ensure only one instance of a configuration is grabbed from the factory.
        
        public ConfigurationFactory()
        {
            if (_instance != null)
            {
                throw new Exception("DOUBLE FACTORY OH NO");
            }
            _backedConfigurations = new Dictionary<string, IConfiguration>();
        }

        public IConfiguration GetConfiguration(string ClassType)
        {
            if (_backedConfigurations.ContainsKey(ClassType))
            {
                return _backedConfigurations[ClassType];
            }

            IConfiguration config = new JsonConfiguration(ClassType + ".json") as IConfiguration;
            _backedConfigurations.Add(ClassType, config);
            return config;
        }
    }
}
