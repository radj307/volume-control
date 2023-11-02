namespace VolumeControl.Log.Helpers
{
    /// <summary>
    /// Manages a background thread to execute queued actions.
    /// </summary>
    public abstract class ThreadedActionQueue : IDisposable
    {
        #region Constructor
        /// <summary>
        /// Instantiates a new <see cref="ThreadedActionQueue"/> instance.
        /// </summary>
        protected ThreadedActionQueue()
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

        #region Methods

        #region (Private) ProcessQueue
        private void ProcessQueue()
        {
            var waitHandles = new WaitHandle[] { _itemAddedSignal, _terminateSignal };
            Queue<Action> queueCopy;
            while (true)
            {
                _isWaitingSignal.Set();
                int i = WaitHandle.WaitAny(waitHandles);

                if (i == 1) return; //< terminate was signaled

                _isWaitingSignal.Reset();
                _itemAddedSignal.Reset();

                lock (_queue)
                {
                    queueCopy = new(_queue);
                    _queue.Clear();
                }

                foreach (var action in queueCopy)
                {
                    action.Invoke();
                }
            }
        }
        #endregion (Private) ProcessQueue

        #region (Protected) Enqueue
        /// <summary>
        /// Adds the specified <paramref name="action"/> to the queue.
        /// </summary>
        /// <param name="action">A delegate to add to the action queue.</param>
        protected void Enqueue(Action action)
        {
            lock (_queue)
            {
                _queue.Enqueue(action);
            }
            _itemAddedSignal.Set();
        }
        #endregion (Protected) Enqueue

        #region Flush
        /// <summary>
        /// Blocks the caller until the background thread has finished processing the queue.
        /// </summary>
        public void Flush()
            => _isWaitingSignal.WaitOne();
        /// <summary>
        /// Blocks the caller until the background thread has finished processing the queue, or until the specified timeout has elapsed.
        /// </summary>
        /// <param name="timeoutMs">How many milliseconds to wait before returning early, or (-1) to wait indefinitely.</param>
        /// <returns><see langword="true"/> when the queue was successfully flushed before timing out; otherwise <see langword="false"/>.</returns>
        public bool Flush(int timeoutMs)
            => _isWaitingSignal.WaitOne(timeoutMs);
        /// <summary>
        /// Blocks the caller until the background thread has finished processing the queue, or until the specified timeout has elapsed.
        /// </summary>
        /// <param name="timeoutMs">How many milliseconds to wait before returning early, or (-1) to wait indefinitely.</param>
        /// <param name="exitContext"><see langword="true"/> to exit the synchronization domain for the context before waiting &amp; reacquire it afterwards; otherwise, <see langword="false"/>.<br/>For more information, see <see href="https://stackoverflow.com/a/755629/8705305">this StackOverflow answer</see>.</param>
        /// <returns><see langword="true"/> when the queue was successfully flushed before timing out; otherwise <see langword="false"/>.</returns>
        public bool Flush(int timeoutMs, bool exitContext)
            => _isWaitingSignal.WaitOne(timeoutMs, exitContext);
        #endregion Flush

        #endregion Methods

        #region IDisposable Implementation
        /// <summary>
        /// Calls <see cref="Dispose"/>.
        /// </summary>
        ~ThreadedActionQueue() => Dispose();
        /// <summary>
        /// Terminates the background thread. Does not flush the queue.
        /// </summary>
        public void Dispose()
        {
            _terminateSignal.Set();
            _thread.Join();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
