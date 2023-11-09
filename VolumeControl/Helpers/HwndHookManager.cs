using System;
using System.Collections.Generic;
using System.Windows.Interop;
using VolumeControl.Core.Helpers;
using VolumeControl.Log.Interfaces;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers
{
    /// <summary>
    /// Helper class that manages <see cref="HwndSourceHook"/> delegates for a specific <see cref="HwndSource"/>.
    /// </summary>
    public class HwndSourceHookManager : IDisposable
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="HwndSourceHookManager"/> instance for the specified <paramref name="hWndSource"/>.
        /// </summary>
        /// <param name="hWndSource"></param>
        public HwndSourceHookManager(HwndSource hWndSource, ILogWriter? logWriter)
        {
            _hwndSource = hWndSource;
            Log = logWriter;
            Log?.Debug($"[{nameof(HwndSourceHookManager)}] Created hook manager for window handle \"{_hwndSource.Handle}\".");
        }
        #endregion Constructor

        #region Fields
        private readonly HwndSource _hwndSource;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets a value that indicates whether the Source's handle is <see langword="null"/> or not.
        /// </summary>
        public bool IsSourceHandleNull => _hwndSource.Handle != IntPtr.Zero;
        /// <summary>
        /// Gets the list of attached hooks.
        /// </summary>
        public IReadOnlyCollection<HwndSourceHook> SourceHooks => _sourceHooks;
        private readonly HashSet<HwndSourceHook> _sourceHooks = new();
        public ILogWriter? Log { get; set; }
        #endregion Properties

        #region Events
        /// <summary>
        /// Occurs when a source hook is added to this manager instance for any reason.
        /// </summary>
        public event EventHandler<HwndSourceHook>? HookAdded;
        private void NotifyHookAdded(HwndSourceHook addedHook) => HookAdded?.Invoke(this, addedHook);
        /// <summary>
        /// Occurs when a source hook is removed from this manager instance for any reason.
        /// </summary>
        public event EventHandler<HwndSourceHook>? HookRemoved;
        private void NotifyHookRemoved(HwndSourceHook removedHook) => HookRemoved?.Invoke(this, removedHook);
        #endregion Events

        #region Methods

        #region GetWindowHandle
        /// <summary>
        /// Gets the window handle that this <see cref="HwndSourceHookManager"/> instance is attached to.
        /// </summary>
        /// <returns>The handle of the <see cref="HwndSource"/> as an <see cref="IntPtr"/>.</returns>
        public IntPtr GetWindowHandle() => _hwndSource.Handle;
        #endregion GetWindowHandle

        #region (Private) SourceHookToString
        private static string SourceHookToString(HwndSourceHook sourceHook) => $"{{ Method: \"{sourceHook.Method.GetFullMethodName()}\", Target: \"{sourceHook.Target}\" }}";
        #endregion (Private) SourceHookToString

        #region (Private) AttachHook
        private void AttachHook(HwndSourceHook sourceHook)
        {
            _hwndSource.AddHook(sourceHook);
            Log?.Debug($"[{nameof(HwndSourceHookManager)}] Attached source hook: {SourceHookToString(sourceHook)}");
        }
        #endregion (Private) AttachHook

        #region (Private) AttachAllHooks
        private void AttachAllHooks() => SourceHooks.ForEach(sourceHook => AttachHook(sourceHook));
        #endregion (Private) AttachAllHooks

        #region (Private) DetatchHook
        private void DetatchHook(HwndSourceHook sourceHook)
        {
            _hwndSource.RemoveHook(sourceHook);
            Log?.Debug($"[{nameof(HwndSourceHookManager)}] Detatched source hook: {SourceHookToString(sourceHook)}");
        }
        #endregion (Private) DetatchHook

        #region (Private) DetatchAllHooks
        private void DetatchAllHooks() => SourceHooks.ForEach(sourceHook => DetatchHook(sourceHook));
        #endregion (Private) DetatchAllHooks

        #region AddHook
        public void AddHook(HwndSourceHook sourceHook, bool attachHook = true)
        {
            if (IsSourceHandleNull)
            {
                Log?.Debug($"[{nameof(HwndSourceHookManager)}] Window handle is null; cannot add source hook: {SourceHookToString(sourceHook)}");
                return;
            }

            _sourceHooks.Add(sourceHook);

            NotifyHookAdded(sourceHook);
        }
        #endregion AddHook

        #region AddHooks
        public void AddHooks(IEnumerable<HwndSourceHook> sourceHooks)
        {
            foreach (var sourceHook in sourceHooks)
            {
                AddHook(sourceHook);
            }
        }
        public void AddHooks(params HwndSourceHook[] sourceHooks) => AddHooks((IEnumerable<HwndSourceHook>)sourceHooks);
        #endregion AddHooks

        #region RemoveHook
        /// <summary>
        /// Removes the specified <paramref name="sourceHook"/> from this manager instance.
        /// </summary>
        /// <param name="sourceHook"></param>
        /// <returns><see langword="true"/> when <paramref name="sourceHook"/> was found and successfully removed; otherwise, <see langword="false"/>.</returns>
        public bool RemoveHook(HwndSourceHook sourceHook)
        {
            if (IsSourceHandleNull)
            {
                Log?.Debug($"[{nameof(HwndSourceHookManager)}] Window handle is null; cannot remove source hook: {SourceHookToString(sourceHook)}");
                return false;
            }

            if (_sourceHooks.Remove(sourceHook))
            {
                DetatchHook(sourceHook);
                NotifyHookRemoved(sourceHook);
                return true;
            }
            else return false;
        }
        #endregion RemoveHook

        #endregion Methods

        #region IDisposable
        /// <inheritdoc cref="Dispose"/>
        ~HwndSourceHookManager() => Dispose();
        /// <summary>
        /// Detatches all source hooks.
        /// </summary>
        /// <remarks>
        /// Does not dispose of the underlying <see cref="HwndSource"/> to prevent breaking the underlying window.
        /// </remarks>
        public void Dispose()
        {
            DetatchAllHooks();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }

}
