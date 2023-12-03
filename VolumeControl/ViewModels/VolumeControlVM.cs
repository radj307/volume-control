using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Core.Input;
using VolumeControl.CoreAudio;
using VolumeControl.Helpers;
using VolumeControl.Helpers.Update;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.SDK.Internal;

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
            AddonLoader = new();
            AddonLoader.LoadAddons(this);

            // Retrieve a sorted list of all loaded actions
            var sortedActionDefinitions = HotkeyAPI.HotkeyManager.HotkeyActionManager.ActionDefinitions
                .OrderBy(a => a.GroupName)
                .ThenBy(a => a.Name);
            // Populate the action definitions list
            foreach (var actionDefinition in sortedActionDefinitions)
            {
                _actions.Add(new(actionDefinition));
            }

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
        public readonly UpdateChecker Updater;
        #endregion Fields

        #region Properties
        #region Other
        /// <summary>
        /// This is used by the target box's autocomplete feature, and is automatically invalidated & refreshed each time the sessions list changes.
        /// </summary>
        public IEnumerable<string> AudioSessionProcessIdentifierAutocompleteSource { get; private set; } = null!;
        public IEnumerable<string> AudioSessionProcessNameAutocompleteSource { get; private set; } = null!;
        public IReadOnlyList<ActionVM> Actions => _actions;
        private readonly List<ActionVM> _actions = new();
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

        public AudioDeviceManagerVM AudioAPI { get; }
        public HotkeyManagerVM HotkeyAPI { get; }
        public AddonLoader AddonLoader { get; }
        public TargetBoxVM TargetBoxVM { get; }
        public NotificationConfigSectionVM SessionConfigVM { get; }
        public NotificationConfigSectionVM DeviceConfigVM { get; }
        public bool IsNotificationConfigAdditionalSettingsExpanderOpen { get; set; } = false;
        #endregion Properties

        #region Methods
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

        #region EventHandlers
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
        #endregion EventHandlers

        #region IDisposable Implementation
        ~VolumeControlVM() => Dispose();
        /// <inheritdoc/>
        public override void Dispose()
        {
            HotkeyAPI?.Dispose();
            AudioAPI?.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
