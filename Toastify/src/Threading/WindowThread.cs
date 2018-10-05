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

        public Thread Thread { get; }

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
                logger.Error($"Unhandled error while closing {nameof(WindowThread<T>)}'s window{(!string.IsNullOrWhiteSpace(this.Window?.Title) ? $" ({this.Window.Title})" : string.Empty)}", ex);
            }
        }

        private void ThreadStart()
        {
            try
            {
                var syncContext = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                this.Window = new T();
                this.options?.WindowInitialization?.Invoke(this.Window);

                this.Window.Closed += (sender, args) =>
                {
                    Application.Current?.Dispatcher?.BeginInvoke(new Action(() => Application.Current.Exit -= this.Application_Exit));
                    this.options?.OnWindowClosingAction?.Invoke(this);
                    this.Window?.Dispatcher?.BeginInvokeShutdown(DispatcherPriority.Background);
                };
                Application.Current?.Dispatcher?.Invoke(() => Application.Current.Exit += this.Application_Exit);

                this.options?.BeforeWindowShownAction?.Invoke(this.Window);
                this.Window.Show();
                this.options?.AfterWindowShownAction?.Invoke(this.Window);

                Dispatcher.Run();
                SynchronizationContext.SetSynchronizationContext(syncContext);
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                logger.Error($"Unhandled error in {nameof(WindowThread<T>)}{(!string.IsNullOrWhiteSpace(this.ThreadName) ? $" ({this.ThreadName})" : string.Empty)}", ex);
            }
            finally
            {
                this.CloseWindow();
                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
            }
        }

        public override int GetHashCode()
        {
            return this.Thread?.GetHashCode() ?? 0;
        }

        public void Dispose()
        {
            try
            {
                this.Abort();
            }
            catch
            {
                // ignore
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            this.CloseWindow();
        }
    }
}