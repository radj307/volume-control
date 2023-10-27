using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

            // Create the hotkey manager
            this.HotkeyAPI = new();

            // Initialize the addon API
            var api = Initializer.Initialize(this.AudioAPI.AudioDeviceManager, this.AudioAPI.AudioDeviceSelector, this.AudioAPI.AudioSessionManager, this.AudioAPI.AudioSessionMultiSelector, this.HotkeyAPI.HotkeyManager, this.MainWindowHandle, (AppConfig.Configuration.Default as Config)!);

            var actions = HotkeyActionAddonLoader.Load(GetDefaultActionGroupTypes());

            HotkeyAPI.HotkeyManager.HotkeyActionManager.AddActionDefinitions(actions);

            AddonDirectories.ForEach(dir =>
            {
                if (Directory.Exists(dir))
                {
                    foreach (string dllPath in Directory.EnumerateFiles(dir, "*.dll", new EnumerationOptions()
                    {
                        MatchCasing = MatchCasing.CaseInsensitive,
                        RecurseSubdirectories = false,
                    }))
                    {
                        FLog.Trace($"Found addon DLL \"{dllPath}\"");

                        HotkeyAPI.HotkeyManager.HotkeyActionManager.AddActionDefinitions(HotkeyActionAddonLoader.Load(Assembly.LoadFrom(dllPath).GetExportedTypes()));
                    }
                }
            });

            // Retrieve a list of all loaded action names
            this.Actions = HotkeyAPI.HotkeyManager.HotkeyActionManager.ActionDefinitions
                .OrderBy(a => a.GroupName)
                .ThenBy(a => a.Name)
                .ToList();

            // load saved hotkeys
            //  We need to have accessed the Settings propertyInfo at least once by the time we reach this point
            this.HotkeyAPI.LoadHotkeys();

            // attach event to update TargetSessionText & LockTargetSession properties
            VCAPI.Default.AudioSessionMultiSelector.PropertyChanged += this.AudioSessionSelector_PropertyChanged;

            // setup autocomplete
            RefreshSessionAutoCompleteSources();
            // bind to session added/removed events to update the auto complete sources
            AudioAPI.AudioSessionManager.AddedSessionToList += this.AudioSessionManager_AddedOrRemovedSession;
            AudioAPI.AudioSessionManager.RemovedSessionFromList += this.AudioSessionManager_AddedOrRemovedSession;

            // setup flags viewmodels
            SessionListNotificationFlagsVM = new(Settings.SessionListNotificationConfig);
            DeviceListNotificationFlagsVM = new(Settings.DeviceListNotificationConfig);

            // setup notification brushes
            SessionListNotificationLockedBrush = new SolidColorBrush(Settings.SessionListNotificationConfig.LockedColor);
            SessionListNotificationUnlockedBrush = new SolidColorBrush(Settings.SessionListNotificationConfig.UnlockedColor);
            DeviceListNotificationLockedBrush = new SolidColorBrush(Settings.DeviceListNotificationConfig.LockedColor);
            DeviceListNotificationUnlockedBrush = new SolidColorBrush(Settings.DeviceListNotificationConfig.UnlockedColor);
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
        private static Config Settings => (AppConfig.Configuration.Default as Config)!;
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
        //public AudioDeviceManagerVM AudioDeviceManagerVM { get; }
        public AudioDeviceManagerVM AudioAPI { get; }
        public HotkeyManagerVM HotkeyAPI { get; }
        //public ListNotificationVM ListNotificationVM { get; }
        #endregion ParentObjects

        /// <summary>
        /// Gets or sets the target session text that appears on the mixer tab.
        /// </summary>
        public string TargetSessionText
        {
            get => AudioAPI.AudioSessionMultiSelector.CurrentSession?.ProcessIdentifier ?? string.Empty;
            set
            {
                value = value.Trim();

                if (value.Length > 0)
                {
                    if (AudioAPI.AudioSessionManager.FindSessionWithSimilarProcessIdentifier(value) is AudioSession session)
                    { // text resolves to a valid AudioSession, select it:
                        AudioAPI.AudioSessionMultiSelector.CurrentSession = session;
                    }
                    else
                    { // text does not resolve to a valid AudioSession:
                        // update Settings.TargetSession with the (invalid) process identifier directly.
                        //  This causes the AudioSessionSelector to receive a PropertyChanged event and deselect the previously selected session.
                        //  AudioSessionSelector has code to prevent overwriting Settings.TargetSession in this case.
                        Settings.TargetSession = new() { ProcessName = value };
                    }
                }
                else
                {
                    AudioAPI.AudioSessionMultiSelector.DeselectCurrentItem();
                }
            }
        }

        public ListNotificationViewFlagsVM SessionListNotificationFlagsVM { get; }
        public ListNotificationViewFlagsVM DeviceListNotificationFlagsVM { get; }

        public Brush SessionListNotificationLockedBrush { get; }
        public Brush SessionListNotificationUnlockedBrush { get; }
        public Brush DeviceListNotificationLockedBrush { get; }
        public Brush DeviceListNotificationUnlockedBrush { get; }
        #endregion Properties

        #region Methods

        #region GetDefaultActionGroupTypes
        private static Type[] GetDefaultActionGroupTypes()
        {
            var asm = Assembly.Load($"{nameof(VolumeControl)}.{nameof(HotkeyActions)}");

            return asm.GetExportedTypes().Where(type => type.GetCustomAttribute<Core.Attributes.HotkeyActionGroupAttribute>() != null).ToArray();
        }
        #endregion GetDefaultActionGroupTypes

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

            if (e.PropertyName.Equals(nameof(AudioSessionMultiSelector.CurrentIndex)))
            {
                ForceNotifyPropertyChanged(nameof(TargetSessionText));
            }
            else if (e.PropertyName.Equals(nameof(AudioSessionSelector.LockSelection)))
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
