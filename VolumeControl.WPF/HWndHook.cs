using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Interop;
using VolumeControl.Log;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Handles receiving windows event messages.
    /// </summary>
    public class HWndHook : IDisposable
    {
        /// <inheritdoc cref="HWndHook"/>
        /// <param name="hWndSource">The main window's <see cref="HwndSource"/> object.</param>
        public HWndHook(HwndSource hWndSource) => SetSource(hWndSource);
        /// <inheritdoc cref="HWndHook"/>
        public HWndHook() { }

        private List<HwndSourceHook> Hooks { get; } = new();
        private HwndSource? _source;

        /// <summary>
        /// Gets the current target handle from the hook source.
        /// </summary>
        public IntPtr Handle => _source?.Handle ?? IntPtr.Zero;

        private static LogWriter Log => FLog.Log;

        /// <summary>
        /// Sets the <see cref="HwndSource"/> used to add and remove hooks.
        /// </summary>
        /// <param name="src">A new source object.</param>
        /// <param name="caller">Used for logging purposes.</param>
        public void SetSource(HwndSource src, [CallerMemberName] string caller = "")
        {
            if (_source != null)
            {
                DetachAll();
                Log.Debug($"{caller} is replacing the hook source '{_source.Handle}' with '{src.Handle}'");
            }
            else Log.Debug($"{caller} is setting the hook source to '{src.Handle}' (Was null)");
            _source = src;
            AttachAll();
        }

        /// <summary>
        /// Adds a message handler.
        /// </summary>
        /// <param name="hook">The handler delegate to use.</param>
        /// <param name="caller">Used for logging purposes.</param>
        public void AddHook(HwndSourceHook hook, [CallerMemberName] string caller = "")
        {
            Hooks.Add(hook);
            Attach(hook);
            Log.Debug($"Attached a message hook from '{caller}'.");
        }
        /// <summary>
        /// Removes a message handler.
        /// </summary>
        /// <param name="hook">The hook to remove.</param>
        /// <param name="caller">Used for logging purposes.</param>
        public void RemoveHook(HwndSourceHook hook, [CallerMemberName] string caller = "")
        {
            int i = Hooks.IndexOf(hook);
            if (i != -1)
            {
                var inst = Hooks[i];
                Detach(inst);
                Hooks.RemoveAt(i);
                Log.Debug($"{caller} removed a message hook.");
            }
            else Log.Warning($"{caller} attempted to remove a message hook that doesn't exist!");
        }

        private void Attach(HwndSourceHook hook)
        {
            if (_source == null || _source.Handle.Equals(IntPtr.Zero))
            {
                Log.Error($"Cannot attach {hook.Method} to a null source!");
            }
            else _source.AddHook(hook);
        }
        private void Detach(HwndSourceHook hook)
        {
            if (_source == null || _source.Handle.Equals(IntPtr.Zero))
            {
                Log.Error($"Cannot detach {hook.Method} from a null source!");
            }
            else _source.RemoveHook(hook);
        }

        private void AttachAll() => Hooks.ForEach(hook => Attach(hook));
        private void DetachAll() => Hooks.ForEach(hook => Detach(hook));

        /// <inheritdoc/>
        public void Dispose()
        {
            DetachAll();
            _source?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}