namespace VolumeControl.Log
{
    /// <summary>
    /// Manages a background thread to write queued log messages. The queue can be disabled at runtime to switch between asynchronous and synchronous operation.
    /// </summary>
    public abstract class ThreadedLogger : IDisposable
    {
        #region Constructor
        /// <summary>
        /// Instantiates a new <see cref="ThreadedLogger"/> instance.
        /// </summary>
        public ThreadedLogger()
        {
            _thread = new Thread(new ThreadStart(ProcessQueue)) { IsBackground = true };
            // this is performed from a bg thread, to ensure the queue is serviced from a single thread
            _thread.Start();
        }
        #endregion Constructor

        #region Fields
        private readonly Queue<Action> _queue = new();
        private readonly ManualResetEvent _itemAddedSignal = new(false);
        private readonly ManualResetEvent _terminateSignal = new(false);
        private readonly ManualResetEvent _isWaitingSignal = new(false);
        private readonly Thread _thread;
        #endregion Fields

        #region Thread Method
        private void ProcessQueue()
        {
            var waitHandles = new WaitHandle[]
            {
                _itemAddedSignal,
                _terminateSignal
            };
            while (true)
            {
                _isWaitingSignal.Set();
                int i = WaitHandle.WaitAny(waitHandles);

                if (i == 1) return; //< terminate was signaled

                _itemAddedSignal.Reset();
                _isWaitingSignal.Reset();

                Queue<Action> queueCopy;
                lock (_queue)
                {
                    queueCopy = new Queue<Action>(_queue);
                    _queue.Clear();
                }

                foreach (var log in queueCopy)
                {
                    log();
                }
            }
        }
        #endregion Thread Method

        #region Methods
        /// <summary>
        /// Writes the specified <paramref name="message"/> to the log endpoint.
        /// </summary>
        /// <param name="message">The log message instance to write to the endpoint.</param>
        protected abstract void WriteLogMessage(LogMessage message);
        /// <summary>
        /// Adds the specified <paramref name="message"/> to the message queue when UseAsyncQueue is <see langword="true"/>; otherwise writes the message synchronously.
        /// </summary>
        /// <param name="message">A log message instance.</param>
        protected void LogMessage(LogMessage message)
        {
            message.RemoveNullLines(); //< remove all null lines
            if (message.IsEmpty) return;

            lock (_queue)
            {
                _queue.Enqueue(() => WriteLogMessage(message));
            }
            _itemAddedSignal.Set();
        }
        /// <summary>
        /// Flushes the message queue.
        /// </summary>
        public void Flush()
        {
            _isWaitingSignal.WaitOne();
        }
        #endregion Methods

        #region IDisposable Implementation
        bool disposed = false;
        /// <summary>
        /// Disposes of this <see cref="ThreadedLogger"/> instance, if it hasn't already been disposed.
        /// </summary>
        ~ThreadedLogger()
        {
            if (!disposed) Dispose();
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            disposed = true;
            _terminateSignal.Set();
            _thread.Join();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
