using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using NLog;

namespace DOL.Logging
{
    public static class LogMessageQueue
    {
        private static Thread _thread;
        private static BlockingCollection<LogMessage> _loggingQueue;
        private static CancellationTokenSource _cancellationTokenSource;
        private static Logger _logger;
        private static Lock _lock = new();

        public static void Start()
        {
            lock (_lock)
            {
                if (_thread != null)
                    return;

                _logger = LoggingManager.Create(MethodBase.GetCurrentMethod().DeclaringType);
                _cancellationTokenSource = new();
                _loggingQueue = new(new ConcurrentQueue<LogMessage>(), 1000);
                _thread = new Thread(new ThreadStart(Run))
                {
                    Name = nameof(LogMessageQueue),
                    IsBackground = true,
                    Priority = ThreadPriority.Lowest
                };
                _thread.Start();
            }
        }

        public static void Stop()
        {
            lock (_lock)
            {
                if (_thread == null)
                    return;

                if (Thread.CurrentThread != _thread)
                {
                    _cancellationTokenSource.Cancel();
                    _thread.Join();
                }

                _thread = null;
                _loggingQueue.Dispose();
                _loggingQueue = null;
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                _logger = null;
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        public static void EnqueueMessage(in LogMessage logMessage)
        {
            _loggingQueue.TryAdd(logMessage);
        }

        private static void Run()
        {
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _loggingQueue.Take(cancellationToken).Process();
                }
                catch (OperationCanceledException)
                {
                    _logger.Info($"Thread \"{_thread.Name}\" was cancelled");
                }
                catch (ThreadInterruptedException)
                {
                    _logger.Info($"Thread \"{_thread.Name}\" was interrupted");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Critical error encountered in {nameof(LogMessageQueue)}. Disabling logging:{Environment.NewLine}{e}");
                    Stop();
                    return;
                }
            }

            if (_loggingQueue.TryTake(out LogMessage logMessage))
                logMessage.Process();
        }
    }
}
