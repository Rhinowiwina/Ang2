using System;
using Common.Logging;

namespace LS.Utilities
{

    public static class LoggerFactory
    {
        public static ILog GetLogger(Type loggingClassType)
        {
            return LogManager.GetLogger(loggingClassType);
        }

        public static ILog GetLogger(string logName)
        {
            return LogManager.GetLogger(logName);
        }
    }
}
