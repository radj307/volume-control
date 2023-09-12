using CodingSeb.Localization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Core.Input.Actions;
using VolumeControl.CoreAudio;
using VolumeControl.Helpers;
using VolumeControl.Helpers.Addon;
using VolumeControl.Helpers.Update;
using VolumeControl.Hotkeys;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.SDK.Internal;
using VolumeControl.TypeExtensions;

namespace VolumeControl.ViewModels
{
    /// <summary>Inherits from the <see cref="VCSettingsContainer"/> object.</summary>
    public class VolumeControlVM : VCSettings, IDisposable
    {
        public VolumeControlVM() : base()
        {
            // create the updater & check for updates if enabled
            Updater = new(this);
            if (Settings.CheckForUpdates) Updater.CheckForUpdateNow();

            this.AudioAPI = new();
            string? i = Loc.Instance.AvailableLanguages[0];

            // Hotkey Action Addons:
            HotkeyActionManager actionManager = new();

            // Create the hotkey manager
            this.HotkeyAPI = new(actionManager);

            // Initialize the addon API
            var api = Initializer.Initialize(this.AudioAPI.AudioDeviceManager, this.AudioAPI.AudioDeviceSelector, this.AudioAPI.AudioSessionManager, this.AudioAPI.AudioSessionSelector, this.HotkeyAPI, this.MainWindowHandle, (AppConfig.Configuration.Default as Config)!);

            VCHotkeyAddon vcHkAddon = new();

            // Load default types ; types (action groups) are loaded in order of appearance.
            vcHkAddon.LoadTypes(
                typeof(AudioDeviceActions),
                typeof(AudioSessionActions),
                typeof(ActiveApplicationActions),
                typeof(ApplicationActions),
                typeof(MediaActions)
            );

            AddonDirectories.ForEach(dir =>
            {
                if (Directory.Exists(dir))
                {
                    foreach (string dll in Directory.EnumerateFiles(dir, "*.dll", new EnumerationOptions()
                    {
                        MatchCasing = MatchCasing.CaseInsensitive,
                        RecurseSubdirectories = false,
                    }))
                    {
                        vcHkAddon.LoadAssemblyFrom(dll);
                    }
                }
            });

            // Retrieve a list of all loaded action names
            const char sort_last = (char)('z' + 1);
            this.Actions = actionManager
                .Bindings
                .OrderBy(a => a.Data.ActionName.AtIndexOrDefault(0, sort_last))                                           //< sort entries alphabetically
                .OrderBy(a => a.Data.ActionGroup is null ? sort_last : a.Data.ActionGroup.AtIndexOrDefault(0, sort_last)) //< sort entries by group; null groups are always last.
                .ToList();

            // load saved hotkeys
            //  We need to have accessed the Settings propertyInfo at least once by the time we reach this point
            this.HotkeyAPI.LoadHotkeys();

            Log.Info($"Volume Control v{this.CurrentVersionString}");

            // attach event to update TargetSessionText & LockTargetSession properties
            VCAPI.Default.AudioSessionSelector.PropertyChanged += this.AudioSessionSelector_PropertyChanged;

            // setup autocomplete
            RefreshSessionAutoCompleteSources();
            // bind to session added/removed events to update the auto complete sources
            AudioAPI.AudioSessionManager.SessionAddedToList += this.AudioSessionManager_SessionAddedOrRemoved;
            AudioAPI.AudioSessionManager.SessionRemovedFromList += this.AudioSessionManager_SessionAddedOrRemoved;
        }

        #region Events
        public override event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add
            {
                this.HotkeyAPI.CollectionChanged += value;
                this.CustomAddonDirectories.CollectionChanged += value;
                this.CustomLocalizationDirectories.CollectionChanged += value;
            }
            remove
            {
                this.HotkeyAPI.CollectionChanged -= value;
                this.CustomAddonDirectories.CollectionChanged += value;
                this.CustomLocalizationDirectories.CollectionChanged += value;
            }
        }
        #endregion Events

        #region Fields
        #region PrivateFields
        private bool disposedValue;
        private bool _updatingAudioSessionSelectorFromTargetSessionText = false;
        #endregion PrivateFields
        public readonly Updater Updater;
        #endregion Fields

        #region Properties
        #region Other
        /// <summary>
        /// This is used by the target box's autocomplete feature, and is automatically invalidated & refreshed each time the sessions list changes.
        /// </summary>
        public IEnumerable<string> AudioSessionProcessIdentifierAutocompleteSource { get; private set; }
        public IEnumerable<string> AudioSessionProcessNameAutocompleteSource { get; private set; }
        public IEnumerable<IHotkeyAction> Actions { get; internal set; } = null!;
        public IEnumerable<string> AddonDirectories { get; set; } = GetAddonDirectories();
        #endregion Other

        #region Statics
        #region PrivateStatics
        /// <summary>Static accessor for <see cref="Settings.Default"/>.</summary>
        private static Config Settings => (AppConfig.Configuration.Default as Config)!;
        private static LogWriter Log => FLog.Log;
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
        public HotkeyManager HotkeyAPI { get; }
        //public ListNotificationVM ListNotificationVM { get; }
        #endregion ParentObjects

        /// <summary>
        /// Gets or sets the target session text that appears on the mixer tab.
        /// </summary>
        public string TargetSessionText
        {
            get => AudioAPI.AudioSessionSelector.Selected?.ProcessIdentifier ?? Settings.Target.ProcessIdentifier;
            set
            {
                value = value.Trim();

                _updatingAudioSessionSelectorFromTargetSessionText = true;

                if (value.Length > 0)
                {
                    if (AudioAPI.AudioSessionManager.FindSessionWithProcessIdentifier(value) is AudioSession session)
                    { // text resolves to a valid AudioSession, select it:
                        AudioAPI.AudioSessionSelector.Selected = session;
                    }
                    else
                    { // text does not resolve to a valid AudioSession:
                        // update Settings.Target with the (invalid) process identifier directly.
                        //  This causes the AudioSessionSelector to receive a PropertyChanged event and deselect the previously selected session.
                        //  AudioSessionSelector has code to prevent overwriting Settings.Target in this case.
                        Settings.Target = new() { ProcessIdentifier = value };
                    }
                }
                else
                {
                    AudioAPI.AudioSessionSelector.Selected = null;
                }

                _updatingAudioSessionSelectorFromTargetSessionText = false;
            }
        }
        #endregion Properties

        #region Methods
        public static string GetExecutablePath()
        {
            AppDomain? appDomain = AppDomain.CurrentDomain;
            return Path.Combine(appDomain.RelativeSearchPath ?? appDomain.BaseDirectory, Path.ChangeExtension(appDomain.FriendlyName, ".exe"));
        }
        /// <summary>Displays a message box prompting the user for confirmation, and if confirmation is given, resets all hotkeys to their default settings using <see cref="HotkeyManager.ResetHotkeys"/>.</summary>
        public void ResetHotkeySettings()
        {
            if (MessageBox.Show("Are you sure you want to reset your hotkeys to their default values?\n\nThis cannot be undone!", "Reset Hotkeys?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                this.HotkeyAPI.ResetHotkeys();

                Log.Info("Hotkey definitions were reset to default.");
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
                        Log.Debug($"Successfully added custom addon search directory '{path}'");
                    }
                    else
                    {
                        Log.Debug($"'{nameof(Settings.CustomAddonDirectories)}' contains an item that wasn't found: '{path}'!");
                    }
                }
            }
            else
            {
                Log.Debug($"{nameof(Settings.CustomAddonDirectories)} is null.");
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
                pnames.Add(item.ProcessName);

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

            if (e.PropertyName.Equals(nameof(AudioSessionSelector.Selected)))
            {
                if (!_updatingAudioSessionSelectorFromTargetSessionText)
                {
                    ForceNotifyPropertyChanged(nameof(TargetSessionText));
                }
            }
            else if (e.PropertyName.Equals(nameof(AudioSessionSelector.LockSelection)))
            {
                LockTargetSession = VCAPI.Default.AudioSessionSelector.LockSelection;
            }
        }
        /// <summary>
        /// Refreshes the <see cref="AudioSessionProcessIdentifierAutocompleteSource"/> list.
        /// </summary>
        private void AudioSessionManager_SessionAddedOrRemoved(object? sender, AudioSession e)
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
