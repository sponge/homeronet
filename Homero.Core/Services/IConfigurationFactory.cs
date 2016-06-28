using System;
using System.Collections.Generic;
using Homero.Core.Services.Configuration;

namespace Homero.Core.Services
{
    public interface IConfigurationFactory
    {
        IConfiguration GetConfiguration(string ClassType);
    }

    public class ConfigurationFactory : IConfigurationFactory
    {
        private Dictionary<string, IConfiguration> _backedConfigurations;
            // Ensure only one instance of a configuration is grabbed from the factory.

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
            if (ClassType == null)
            {
                ClassType = "Core";
            }

            if (_backedConfigurations.ContainsKey(ClassType))
            {
                return _backedConfigurations[ClassType];
            }

            IConfiguration config = new JsonConfiguration(ClassType + ".json");
            _backedConfigurations.Add(ClassType, config);
            return config;
        }

        #region Singleton

        private static ConfigurationFactory _instance;

        public static ConfigurationFactory Instance
        {
            get { return _instance ?? (_instance = new ConfigurationFactory()); }
        }

        #endregion
    }
}