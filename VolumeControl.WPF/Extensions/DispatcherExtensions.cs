using System;

namespace VolumeControl.WPF.Extensions
{
    public static class DispatcherExtensions
    {
        public static void InvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, Action callback)
        {
            if (dispatcher.CheckAccess()) callback();
            else dispatcher.Invoke(callback);
        }
        public static void InvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, Action callback, System.Windows.Threading.DispatcherPriority priority)
        {
            if (dispatcher.CheckAccess()) callback();
            else dispatcher.Invoke(callback, priority);
        }
        public static void InvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, Action callback, System.Windows.Threading.DispatcherPriority priority, System.Threading.CancellationToken cancellationToken)
        {
            if (dispatcher.CheckAccess()) callback();
            else dispatcher.Invoke(callback, priority, cancellationToken);
        }
        public static void InvokeIfRequired(this System.Windows.Threading.Dispatcher dispatcher, Action callback, System.Windows.Threading.DispatcherPriority priority, System.Threading.CancellationToken cancellationToken, TimeSpan timeout)
        {
            if (dispatcher.CheckAccess()) callback();
            else dispatcher.Invoke(callback, priority, cancellationToken, timeout);
        }
        public static TResult InvokeIfRequired<TResult>(this System.Windows.Threading.Dispatcher dispatcher, Func<TResult> callback)
        {
            if (dispatcher.CheckAccess()) return callback();
            else return dispatcher.Invoke(callback);
        }
    }
}
