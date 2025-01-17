using System;

namespace DOL.Logging
{
    public abstract class LogMessage
    {
        public Logger Logger { get; init; }
        public Level Level { get; init; }
        public string Content { get; protected set; }

        public void Process()
        {
            BuildContent();
            Logger.ProcessLogMessage(this);
        }

        public static LogMessage Create(Logger logger, Level level, string content)
        {
            return new SimpleLogMessage(logger, level, content);
        }

        public static LogMessage Create(Logger logger, Level level, string content, Exception e)
        {
            return new LogMessageWithException(logger, level, content, e);
        }

        public static LogMessage Create(Logger logger, Level level, string content, params object[] args)
        {
            return new LogMessageWithArgs(logger, level, content, args);
        }

        protected abstract void BuildContent();

        private class SimpleLogMessage : LogMessage
        {
            public SimpleLogMessage(Logger logger, Level level, string content)
            {
                Logger = logger;
                Level = level;
                Content = content;
            }

            protected override void BuildContent() { }
        }

        private class LogMessageWithException : SimpleLogMessage
        {
            public Exception E { get; }

            public LogMessageWithException(Logger logger, Level level, string content, Exception e) : base(logger, level, content)
            {
                E = e;
            }

            protected override void BuildContent()
            {
                Content = $"{Content}{Environment.NewLine}{E}";
            }
        }

        private class LogMessageWithArgs : SimpleLogMessage
        {
            public object[] Args { get; }

            public LogMessageWithArgs(Logger logger, Level level, string content, params object[] args) : base(logger, level, content)
            {
                Args = args;
            }

            protected override void BuildContent()
            {
                Content = string.Format(Content, Args);
            }
        }
    }
}
