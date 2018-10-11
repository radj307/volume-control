using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace Toastify.Threading
{
    public sealed class WindowThread<T> : IWindowThread where T : Window, new()
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(WindowThread<T>));

        private readonly WindowThreadOptions<T> options;

        #region Public Properties

        public T Window { get; private set; }

        public bool IsClosed { get; private set; }

        public Thread Thread { get; private set; }

        public bool IsBackground
        {
            get { return this.Thread?.IsBackground ?? false; }
            set
            {
                if (this.Thread != null)
                    this.Thread.IsBackground = value;
            }
        }

        public string ThreadName
        {
            get { return this.Thread?.Name; }
            set
            {
                if (this.Thread != null)
                    this.Thread.Name = value;
            }
        }

        public Dispatcher Dispatcher
        {
            get { return this.Window?.Dispatcher ?? (this.Thread != null ? Dispatcher.FromThread(this.Thread) : null); }
        }

        #endregion

        public WindowThread(ApartmentState apartmentState) : this(apartmentState, null)
        {
        }

        public WindowThread(ApartmentState apartmentState, WindowThreadOptions<T> options)
        {
            this.Thread = new Thread(this.ThreadStart);
            this.Thread.SetApartmentState(apartmentState);

            this.options = options;
        }

        public void Start()
        {
            this.Thread?.Start();
        }

        public void Abort()
        {
            ThreadManager.Instance.Abort(this.Thread);
            this.Thread = null;
        }

        public void Join()
        {
            this.Thread?.Join();
        }

        public void Join(TimeSpan timeout)
        {
            this.Thread?.Join(timeout);
        }

        public void Join(int millisecondsTimeout)
        {
            this.Thread?.Join(millisecondsTimeout);
        }

        public void CloseWindow()
        {
            if (this.IsClosed)
                return;

            try
            {
                if (this.Thread != null)
                {
                    if (this.Window != null)
                    {
                        if (this.Window.Dispatcher.CheckAccess())
                            this.Window.Close();
                        else
                            this.Window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(this.Window.Close));
                    }

                    this.Window = null;
                }
            }
            catch (Exception ex)
            {
                if (this.Window != null)
                {
                    this.Window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(() =>
                    {
                        string title = !string.IsNullOrWhiteSpace(this.Window?.Title) ? $" ({this.Window.Title})" : string.Empty;
                        logger.Error($"Unhandled error while closing {nameof(WindowThread<T>)}'s window{title}", ex);
                    }));
                }
                else
                    logger.Error($"Unhandled error while closing {nameof(WindowThread<T>)}'s window", ex);
            }
        }

        private void ThreadStart()
        {
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

            try
            {
                this.Window = new T();
                this.options?.WindowInitialization?.Invoke(this.Window);

                this.Window.Closed += (sender, args) =>
                {
                    this.IsClosed = true;
                    try
                    {
                        Application.Current?.Dispatcher?.BeginInvoke(new Action(() => Application.Current.Exit -= this.Application_Exit));
                        this.options?.OnWindowClosingAction?.Invoke(this);
                        this.Window?.Dispatcher?.BeginInvokeShutdown(DispatcherPriority.Background);
                    }
                    catch
                    {
                        // ignore
                    }
                };
                Application.Current?.Dispatcher?.Invoke(() => Application.Current.Exit += this.Application_Exit);

                this.options?.BeforeWindowShownAction?.Invoke(this.Window);
                this.Window.Show();
                this.options?.AfterWindowShownAction?.Invoke(this.Window);

                Dispatcher.Run();
            }
            catch (ThreadAbortException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                logger.Error($"Unhandled error in {nameof(WindowThread<T>)}{(!string.IsNullOrWhiteSpace(this.ThreadName) ? $" ({this.ThreadName})" : string.Empty)}", ex);
            }
            finally
            {
                this.CloseWindow();
                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                SynchronizationContext.SetSynchronizationContext(syncContext);
            }
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.Thread?.GetHashCode() ?? 0;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            this.CloseWindow();
        }

        #region Dispose

        public void Dispose()
        {
            this.Dispose(TimeSpan.FromSeconds(1));
        }

        public void Dispose(TimeSpan timeout)
        {
            try
            {
                this.CloseWindow();
                this.Join(timeout);
            }
            finally
            {
                this.Abort();
            }
        }

        #endregion
    }
}