using System;

namespace DOL.Logging
{
    public abstract class Logger
    {
        public abstract bool IsDebugEnabled { get; }
        public abstract bool IsInfoEnabled { get; }
        public abstract bool IsWarnEnabled { get; }
        public abstract bool IsErrorEnabled { get; }
        public abstract bool IsFatalEnabled { get; }

        public abstract void ProcessLogMessage(LogMessage logMessage);

        public void Trace(string message)
        {
            EnqueueMessage(Level.Trace, message);
        }

        public void Trace(Exception e)
        {
            EnqueueMessage(Level.Trace, e);
        }

        public void Trace(string message, Exception e)
        {
            EnqueueMessage(Level.Trace, message, e);
        }

        public void TraceFormat(string message, params object[] args)
        {
            EnqueueMessage(Level.Trace, message, args);
        }

        public void Info(string message)
        {
            EnqueueMessage(Level.Info, message);
        }

        public void Info(Exception e)
        {
            EnqueueMessage(Level.Info, e);
        }

        public void Info(string message, Exception e)
        {
            EnqueueMessage(Level.Info, message, e);
        }

        public void InfoFormat(string message, params object[] args)
        {
            EnqueueMessage(Level.Info, message, args);
        }

        public void Debug(string message)
        {
            EnqueueMessage(Level.Debug, message);
        }

        public void Debug(Exception e)
        {
            EnqueueMessage(Level.Debug, e);
        }

        public void Debug(string message, Exception e)
        {
            EnqueueMessage(Level.Debug, message, e);
        }

        public void DebugFormat(string message, params object[] args)
        {
            EnqueueMessage(Level.Debug, message, args);
        }

        public void Warn(string message)
        {
            EnqueueMessage(Level.Warning, message);
        }

        public void Warn(Exception e)
        {
            EnqueueMessage(Level.Warning, e);
        }

        public void Warn(string message, Exception e)
        {
            EnqueueMessage(Level.Warning, message, e);
        }

        public void WarnFormat(string message, params object[] args)
        {
            EnqueueMessage(Level.Warning, message, args);
        }

        public void Error(string message)
        {
            EnqueueMessage(Level.Error, message);
        }

        public void Error(Exception e)
        {
            EnqueueMessage(Level.Error, e);
        }

        public void Error(string message, Exception e)
        {
            EnqueueMessage(Level.Error, message, e);
        }

        public void ErrorFormat(string message, params object[] args)
        {
            EnqueueMessage(Level.Error, message, args);
        }

        public void Fatal(string message)
        {
            EnqueueMessage(Level.Fatal, message);
        }

        public void Fatal(Exception e)
        {
            EnqueueMessage(Level.Fatal, e);
        }

        public void Fatal(string message, Exception e)
        {
            EnqueueMessage(Level.Fatal, message, e);
        }

        public void FatalFormat(string message, params object[] args)
        {
            EnqueueMessage(Level.Fatal, message, args);
        }

        private void EnqueueMessage(Level level, string message)
        {
            LogMessageQueue.EnqueueMessage(LogMessage.Create(this, level, message));
        }

        private void EnqueueMessage(Level level, Exception e)
        {
            LogMessageQueue.EnqueueMessage(LogMessage.Create(this, level, string.Empty, e));
        }

        private void EnqueueMessage(Level level, string message, Exception e)
        {
            LogMessageQueue.EnqueueMessage(LogMessage.Create(this, level, message, e));
        }

        private void EnqueueMessage(Level level, string message, params object[] args)
        {
            LogMessageQueue.EnqueueMessage(LogMessage.Create(this, level, message, args));
        }

        public class NullLogger : Logger
        {
            public override bool IsDebugEnabled => false;
            public override bool IsInfoEnabled => false;
            public override bool IsWarnEnabled => false;
            public override bool IsErrorEnabled => false;
            public override bool IsFatalEnabled => false;

            public NullLogger() { }

            public override void ProcessLogMessage(LogMessage logMessage) { }
        }

        public class NLogLogger : Logger
        {
            private NLog.Logger _logger;

            public override bool IsDebugEnabled => _logger.IsDebugEnabled;
            public override bool IsInfoEnabled => _logger.IsInfoEnabled;
            public override bool IsWarnEnabled => _logger.IsWarnEnabled;
            public override bool IsErrorEnabled => _logger.IsErrorEnabled;
            public override bool IsFatalEnabled => _logger.IsFatalEnabled;

            public NLogLogger(string name)
            {
                _logger = NLog.LogManager.GetLogger(name);
            }

            public override void ProcessLogMessage(LogMessage logMessage)
            {
                switch (logMessage.Level)
                {
                    case Level.Trace:
                    {
                        _logger.Trace(logMessage.Content);
                        break;
                    }
                    case Level.Info:
                    {
                        _logger.Info(logMessage.Content);
                        break;
                    }
                    case Level.Debug:
                    {
                        _logger.Debug(logMessage.Content);
                        break;
                    }
                    case Level.Warning:
                    {
                        _logger.Warn(logMessage.Content);
                        break;
                    }
                    case Level.Error:
                    {
                        _logger.Error(logMessage.Content);
                        break;
                    }
                    case Level.Fatal:
                    {
                        _logger.Fatal(logMessage.Content);
                        break;
                    }
                }
            }
        }
    }
}
