using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using VolumeControl.Audio;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Keyboard.Actions;
using VolumeControl.Helpers.Addon;
using VolumeControl.Helpers.Update;
using VolumeControl.Hotkeys;
using VolumeControl.Hotkeys.Addons;
using VolumeControl.Log;
using VolumeControl.SDK.Internal;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers
{
    /// <summary>Inherits from the <see cref="VCSettingsContainer"/> object.</summary>
    public class VolumeControlSettings : VCSettings, IDisposable
    {
        public VolumeControlSettings() : base()
        {
            // create the updater & check for updates if enabled
            Updater = new(this);
            if (Settings.CheckForUpdates) Updater.CheckNow();

            this.AudioAPI = new();

            AddonManager = new(this);

            // Hotkey Action Addons:
            HotkeyActionManager actionManager = new();
            // Add built-in action container types
            actionManager.Types.Add(typeof(AudioDeviceActions));
            actionManager.Types.Add(typeof(AudioSessionActions));
            actionManager.Types.Add(typeof(ApplicationActions));
            actionManager.Types.Add(typeof(MediaActions));

            // Create the hotkey manager
            this.HotkeyAPI = new(actionManager);

            // Initialize the addon API
            Initializer.Initialize(this.AudioAPI, this.HotkeyAPI, this.MainWindowHandle, (Config.Default as Config)!);

            // Create a list of all addon manager types
            List<IBaseAddon> addonControllers = new()
            {
                actionManager
            };

            // Load addon types into the addon list
            AddonManager.LoadAddons(ref addonControllers);

            // Retrieve a list of all loaded action names
            const char sort_last = (char)('z' + 1);
            this.Actions = actionManager
                .Bindings
                .OrderBy(a => a.Data.ActionName.AtIndexOrDefault(0, sort_last))                                           //< sort entries alphabetically
                .OrderBy(a => a.Data.ActionGroup is null ? sort_last : a.Data.ActionGroup.AtIndexOrDefault(0, sort_last)) //< sort entries by group; null groups are always last.
                .ToList();

            // load saved hotkeys
            //  We need to have accessed the Settings property at least once by the time we reach this point
            this.HotkeyAPI.LoadHotkeys();

            Log.Info($"Volume Control v{this.CurrentVersionString}");
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
        private IEnumerable<string>? _targetAutoCompleteSource;
        #endregion PrivateFields
        public readonly AddonManager AddonManager;
        public readonly Updater Updater;
        #endregion Fields

        #region Properties
        #region Other
        /// <summary>
        /// This is used by the target box's autocomplete feature, and is automatically invalidated & refreshed each time the sessions list changes.
        /// </summary>
        public IEnumerable<string> TargetAutoCompleteSource => _targetAutoCompleteSource ??= this.AudioAPI.GetSessionNames(AudioAPI.SessionNameFormat.ProcessIdentifier | AudioAPI.SessionNameFormat.ProcessName);
        public IEnumerable<IActionBinding> Actions { get; internal set; } = null!;
        public IEnumerable<string> NotificationModes { get; set; } = Enum.GetNames(typeof(DisplayTarget));
        #endregion Other

        #region Statics
        #region PrivateStatics
        /// <summary>Static accessor for <see cref="Settings.Default"/>.</summary>
        private static Config Settings => (Config.Default as Config)!;
        private static Log.LogWriter Log => FLog.Log;
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
        public AudioAPI AudioAPI { get; }
        public HotkeyManager HotkeyAPI { get; }
        #endregion ParentObjects
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
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose of objects
                    this.AudioAPI?.Dispose();
                    this.HotkeyAPI?.Dispose();
                }

                disposedValue = true;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        ~VolumeControlSettings()
        {
            this.Dispose(disposing: true);
        }
        #endregion Methods
    }
}
