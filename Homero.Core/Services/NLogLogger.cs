using NLog;

namespace Homero.Core.Services
{
    public class NLogLogger : ILogger
    {
        private readonly Logger _logger;

        public NLogLogger(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }

        public void Info(object message, string callingMethod = null)
        {
            _logger.Info(message.ToString);
        }

        public void Warn(object message, string callingMethod = null)
        {
            _logger.Warn(message.ToString);
        }

        public void Debug(object message, string callingMethod = null)
        {
            _logger.Debug(message.ToString);
        }

        public void Error(object message, string callingMethod = null)
        {
            _logger.Error(message.ToString);
        }
    }
}