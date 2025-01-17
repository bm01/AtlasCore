using System;
using System.Threading;
using System.Xml;
using NLog;

namespace DOL.Logging
{
    public abstract class LoggingManager
    {
        private static Lock _lock = new();
        private static LoggingLibrary _loggingLibrary;
        private static ILoggerFactory _loggerFactory;
        private static bool _initialized;

        public static bool Initialize(string configurationFilePath)
        {
            lock (_lock)
            {
                if (_initialized)
                    throw new InvalidOperationException($"Logging Manager has already been initialized (file: {configurationFilePath}) (library: {_loggingLibrary})");

                try
                {
                    _loggingLibrary = GuessLibrary();
                    Console.WriteLine($"Initializing Logging Manager (file: '{configurationFilePath}') (library: '{_loggingLibrary}')");

                    _loggerFactory = _loggingLibrary switch
                    {
                        LoggingLibrary.NLog => new NLogLoggerFactory(configurationFilePath),
                        LoggingLibrary.Log4net => new Log4netLoggerFactory(configurationFilePath),
                        _ => new NullLoggerFactory(),
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Couldn't initialize Logging Manager (path: {configurationFilePath})", e);
                    return false;
                }

                _initialized = true;
            }

            LogMessageQueue.Start();
            return true;

            LoggingLibrary GuessLibrary()
            {
                using XmlReader reader = XmlReader.Create(configurationFilePath);

                while (reader.Read())
                {
                    if (reader.NodeType is XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("nlog", StringComparison.OrdinalIgnoreCase))
                            return LoggingLibrary.NLog;

                        if (reader.Name.Equals("log4net", StringComparison.OrdinalIgnoreCase))
                            return LoggingLibrary.Log4net;
                    }
                }

                return LoggingLibrary.None;
            }
        }

        public static Logger Create(Type type)
        {
            return Create(type.ToString());
        }

        public static Logger Create(string name)
        {
            if (!_initialized)
                throw new InvalidOperationException("Logging Manager has not been initialized");

            return _loggerFactory.CreateInternal(name);
        }

        public interface ILoggerFactory
        {
            Logger CreateInternal(string name);
        }

        private class NullLoggerFactory : ILoggerFactory
        {
            public Logger CreateInternal(string name)
            {
                return new Logger.NullLogger();
            }
        }

        private class NLogLoggerFactory : ILoggerFactory
        {
            public NLogLoggerFactory(string configurationFilePath)
            {
                LogManager.Setup().LoadConfigurationFromFile(configurationFilePath);
            }

            public Logger CreateInternal(string name)
            {
                return new Logger.NLogLogger(name);
            }
        }

        private class Log4netLoggerFactory : ILoggerFactory
        {
            public Log4netLoggerFactory(string configurationFilePath)
            {
                Console.WriteLine("The 'log4net' logging framework is no longer supported. Please update your configuration to use 'NLog'.");
                Console.WriteLine("You can rename your existing configuration file, and the server will generate a new one compatible with 'NLog'.");
                Console.WriteLine("Note: The log file format may differ. Ensure you archive your existing logs for future reference.");
            }

            public Logger CreateInternal(string name)
            {
                return new Logger.NullLogger();
            }
        }
    }
}
