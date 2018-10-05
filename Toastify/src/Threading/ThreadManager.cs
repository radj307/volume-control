using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace Toastify.Threading
{
    public sealed class ThreadManager : IDisposable
    {
        #region Singleton

        private static ThreadManager _instance;

        public static ThreadManager Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (_instance == null)
                        _instance = new ThreadManager();
                }

                return _instance;
            }
        }

        #endregion

        #region Static Fields and Properties

        private static readonly object instanceLock = new object();

        #endregion

        private readonly HashSet<Thread> threads = new HashSet<Thread>();
        private readonly Thread deadThreadsCleaner;

        private ThreadManager()
        {
            this.deadThreadsCleaner = new Thread(this.DeadThreadsCleanerThreadStart)
            {
                IsBackground = true,
                Name = $"{nameof(ThreadManager)}_CleanerThread"
            };
            this.deadThreadsCleaner.Start();
        }

        public Thread CreateThread(ThreadStart start)
        {
            Thread thread = new Thread(start);
            this.Add(thread);
            return thread;
        }

        public WindowThread<T> CreateWindowThread<T>(ApartmentState apartmentState) where T : Window, new()
        {
            var wt = new WindowThread<T>(apartmentState);
            this.Add(wt.Thread);
            return wt;
        }

        public WindowThread<T> CreateWindowThread<T>(ApartmentState apartmentState, WindowThreadOptions<T> options) where T : Window, new()
        {
            var wt = new WindowThread<T>(apartmentState, options);
            this.Add(wt.Thread);
            return wt;
        }

        public void Add(Thread thread)
        {
            if (thread == null)
                return;
            this.threads.Remove(thread);
            this.threads.Add(thread);
        }

        public void Remove(Thread thread)
        {
            if (thread == null)
                return;
            this.threads.Remove(thread);
        }

        public void Abort(Thread thread)
        {
            this.Abort(thread, true);
        }

        private void Abort(Thread thread, bool remove)
        {
            if (thread == null)
                return;

            try
            {
                if (remove)
                    this.Remove(thread);
                thread.Abort();
            }
            catch
            {
                // ignore
            }
        }

        private void DeadThreadsCleanerThreadStart()
        {
            try
            {
                while (!App.ShutdownEvent.WaitOne(TimeSpan.FromSeconds(10)))
                {
                    List<Thread> threadsToRemove = new List<Thread>(this.threads.Count);
                    foreach (var thread in this.threads)
                    {
                        if (!thread.IsAlive)
                            threadsToRemove.Add(thread);
                    }

                    foreach (var thread in threadsToRemove)
                    {
                        this.Remove(thread);
                    }
                }
            }
            catch
            {
                // ignore
            }
        }

        public void Dispose()
        {
            if (this.threads != null)
            {
                foreach (var thread in this.threads)
                {
                    this.Abort(thread, false);
                }

                this.threads.Clear();
            }

            this.Abort(this.deadThreadsCleaner, false);
        }

        #region Static Members

        public static void DisposeInstance()
        {
            lock (instanceLock)
            {
                if (_instance != null)
                {
                    _instance.Dispose();
                    _instance = null;
                }
            }
        }

        #endregion
    }
}