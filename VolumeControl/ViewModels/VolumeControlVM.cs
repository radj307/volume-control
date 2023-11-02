using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using VolumeControl.Core;
using VolumeControl.Core.Input;
using VolumeControl.Core.Input.Actions;
using VolumeControl.CoreAudio;
using VolumeControl.Helpers;
using VolumeControl.Helpers.Update;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.SDK.Internal;
using VolumeControl.TypeExtensions;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// Viewmodel class for the Volume Control application.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="VCSettings"/>.
    /// </remarks>
    public class VolumeControlVM : VCSettings, IDisposable
    {
        #region Constructor
        public VolumeControlVM() : base()
        {
            // create the updater & check for updates if enabled
            Updater = new(this);
            if (Settings.CheckForUpdates) Updater.CheckForUpdateNow();

            this.AudioAPI = new();

            TargetBoxVM = new(AudioAPI.AudioSessionManager, AudioAPI.AudioSessionMultiSelector);

            // Create the hotkey manager
            this.HotkeyAPI = new();

            // Initialize the addon API
            Initializer.Initialize(this.AudioAPI.AudioDeviceManager, this.AudioAPI.AudioDeviceSelector, this.AudioAPI.AudioSessionManager, this.AudioAPI.AudioSessionMultiSelector, this.HotkeyAPI.HotkeyManager, this.MainWindowHandle, Config.Default!);

            // Load addons
            AddonLoader addonLoader = new();
            addonLoader.LoadAddons(this);

            // Retrieve a sorted list of all loaded action names
            this.Actions = HotkeyAPI.HotkeyManager.HotkeyActionManager.ActionDefinitions
                .OrderBy(a => a.GroupName)
                .ThenBy(a => a.Name)
                .ToList();

            // load saved hotkeys now that actions have been loaded
            this.HotkeyAPI.LoadHotkeys();

            // attach event to update TargetSessionText & LockTargetSession properties
            VCAPI.Default.AudioSessionMultiSelector.PropertyChanged += this.AudioSessionSelector_PropertyChanged;

            // setup autocomplete
            RefreshSessionAutoCompleteSources();
            // bind to session added/removed events to update the auto complete sources
            AudioAPI.AudioSessionManager.AddedSessionToList += this.AudioSessionManager_AddedOrRemovedSession;
            AudioAPI.AudioSessionManager.RemovedSessionFromList += this.AudioSessionManager_AddedOrRemovedSession;

            // setup notification config view models
            SessionConfigVM = new(Settings.SessionListNotificationConfig);
            DeviceConfigVM = new(Settings.DeviceListNotificationConfig);
        }
        #endregion Constructor

        #region Fields
        #region PrivateFields
        private bool disposedValue;
        #endregion PrivateFields
        public readonly UpdateChecker Updater;
        #endregion Fields

        #region Properties
        #region Other
        /// <summary>
        /// This is used by the target box's autocomplete feature, and is automatically invalidated & refreshed each time the sessions list changes.
        /// </summary>
        public IEnumerable<string> AudioSessionProcessIdentifierAutocompleteSource { get; private set; } = null!;
        public IEnumerable<string> AudioSessionProcessNameAutocompleteSource { get; private set; } = null!;
        public IEnumerable<HotkeyActionDefinition> Actions { get; internal set; } = null!;
        public IEnumerable<string> AddonDirectories { get; set; } = GetAddonDirectories();
        #endregion Other

        #region Statics
        #region PrivateStatics
        /// <summary>Static accessor for <see cref="Settings.Default"/>.</summary>
        private static Config Settings => Config.Default;
        #endregion PrivateStatics
        /// <summary>
        /// True when there is a newer version of volume control available.
        /// </summary>
        public bool UpdateAvailable { get; internal set; } = false;
        /// <summary>
        /// The version number of the new version, as a string.
        /// </summary>
        public string UpdateVersion { get; internal set; } = string.Empty;
        #endregion Statics

        #region ParentObjects
        public AudioDeviceManagerVM AudioAPI { get; }
        public HotkeyManagerVM HotkeyAPI { get; }
        #endregion ParentObjects

        public TargetBoxVM TargetBoxVM { get; }
        public NotificationConfigSectionVM SessionConfigVM { get; }
        public NotificationConfigSectionVM DeviceConfigVM { get; }
        public ImageSource KeyboardKeyImageSource { get; } = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.keyboard_key.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        #endregion Properties

        #region Methods

        /// <summary>Displays a message box prompting the user for confirmation, and if confirmation is given, resets all hotkeys to their default settings using <see cref="HotkeyManager.ResetHotkeys"/>.</summary>
        public void ResetHotkeySettings()
        {
            if (MessageBox.Show("Are you sure you want to reset your hotkeys to their default values?\n\nThis cannot be undone!", "Reset Hotkeys?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                this.HotkeyAPI.ResetHotkeys();

                FLog.Info("Hotkey definitions were reset to default.");
            }
        }
        private static List<string> GetAddonDirectories()
        {
            List<string> l = new();
            // check default path:
            string defaultPath = Path.Combine(PathFinder.ApplicationAppDataPath, "Addons");
            if (Directory.Exists(defaultPath))
                _ = l.AddIfUnique(defaultPath);
            // check custom directories:
            if (Settings.CustomAddonDirectories is not null)
            {
                foreach (string? path in Settings.CustomAddonDirectories)
                {
                    if (path is null) continue;
                    if (Directory.Exists(path))
                    {
                        _ = l.AddIfUnique(path);
                        FLog.Debug($"Successfully added custom addon search directory '{path}'");
                    }
                    else
                    {
                        FLog.Debug($"'{nameof(Settings.CustomAddonDirectories)}' contains an item that wasn't found: '{path}'!");
                    }
                }
            }
            else
            {
                FLog.Debug($"{nameof(Settings.CustomAddonDirectories)} is null.");
            }

            return l;
        }
        /// <summary>
        /// Refreshes the list of auto completion options for the target box.
        /// </summary>
        private void RefreshSessionAutoCompleteSources()
        {
            List<string> all = new();
            List<string> pnames = new();

            foreach (var item in AudioAPI.AllSessions)
            {
                pnames.Add(item.Name);

                if (item.AudioSession.HasCustomName)
                    all.Add(item.Name);
                all.Add(item.ProcessIdentifier);
                all.Add(item.ProcessName);
                all.Add(item.PID.ToString());
            }

            AudioSessionProcessIdentifierAutocompleteSource = all;
            AudioSessionProcessNameAutocompleteSource = pnames;
        }
        #endregion Methods

        #region Methods (EventHandlers)
        /// <summary>
        /// Updates <see cref="TargetSessionText"/> &amp; <see cref="TargetSessionLocked"/> with fresh values from <see cref="VCAPI.AudioSessionManager"/>.
        /// </summary>
        private void AudioSessionSelector_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is null) return;

            if (e.PropertyName.Equals(nameof(AudioSessionSelector.LockSelection)))
            {
                LockTargetSession = VCAPI.Default.AudioSessionMultiSelector.LockSelection;
            }
        }
        /// <summary>
        /// Refreshes the <see cref="AudioSessionProcessIdentifierAutocompleteSource"/> list.
        /// </summary>
        private void AudioSessionManager_AddedOrRemovedSession(object? sender, AudioSession e)
            => RefreshSessionAutoCompleteSources();
        #endregion Methods (EventHandlers)

        #region IDisposable Implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    // Dispose of objects
                    this.HotkeyAPI?.Dispose();

                disposedValue = true;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        ~VolumeControlVM()
        {
            this.Dispose(disposing: true);
        }
        #endregion IDisposable Implementation
    }
}
