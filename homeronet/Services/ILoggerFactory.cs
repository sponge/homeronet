using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Services
{
    public interface ILoggerFactory
    {
        ILogger GetLogger(string ClassType);
    }

    public class LoggerFactory : ILoggerFactory
    {
        #region Singleton
        private static LoggerFactory _instance;
        public static LoggerFactory Instance
        {
            get { return _instance ?? (_instance = new LoggerFactory()); }
        }

        #endregion

        private Dictionary<string, ILogger> _backedLoggers; // Ensure only one instance of a configuration is grabbed from the factory.

        public LoggerFactory()
        {
            if (_instance != null)
            {
                throw new Exception("DOUBLE LOGGER FACTORY OH NO");
            }
            _backedLoggers = new Dictionary<string, ILogger>();
        }

        public ILogger GetLogger(string ClassType)
        {
            if (ClassType == null)
            {
                ClassType = "Core";
            }

            if (_backedLoggers.ContainsKey(ClassType))
            {
                return _backedLoggers[ClassType];
            }

            ILogger logger = new NLogLogger(ClassType);
            _backedLoggers.Add(ClassType, logger);
            return logger;
        }
    }
}
