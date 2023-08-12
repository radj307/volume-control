using CodingSeb.Localization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Helpers.Addon;
using VolumeControl.Helpers.Update;
using VolumeControl.Hotkeys;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.SDK.Internal;
using VolumeControl.TypeExtensions;
using VolumeControl.ViewModels;

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
            string? i = Loc.Instance.AvailableLanguages[0];

            // Hotkey Action Addons:
            HotkeyActionManager actionManager = new();

            // Create the hotkey manager
            this.HotkeyAPI = new(actionManager);

            // Initialize the addon API
            var api = Initializer.Initialize(this.AudioAPI.AudioDeviceManager, this.AudioAPI.AudioSessionManager, this.HotkeyAPI, this.MainWindowHandle, (Config.Default as Config)!);

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
            //  We need to have accessed the Settings property at least once by the time we reach this point
            this.HotkeyAPI.LoadHotkeys();

            this.ListNotificationVM = new();

            // Add display targets from VCAPI
            foreach (ListDisplayTarget ldt in VCAPI.Default.ListDisplayTargets)
            {
                this.ListNotificationVM.AddDisplayTarget(ldt);
                Log.Debug($"Added {nameof(ListDisplayTarget)} '{ldt.Name}' from {nameof(VCAPI)}");
            }
            // Sort display targets
            this.ListNotificationVM.DisplayTargets.Sort((l, r) => l.Name.CompareTo(r.Name));
            
            // Set the active display target
            this.ListNotificationVM.SetDisplayTarget(this.ListNotificationVM.FindDisplayTarget(Settings.NotificationDisplayTarget) ?? this.ListNotificationVM.FindDisplayTarget(AudioSessionActions.DisplayTargetName)!);

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
        //public readonly AddonManager AddonManager;
        public readonly Updater Updater;
        #endregion Fields

        #region Properties
        #region Other
        /// <summary>
        /// This is used by the target box's autocomplete feature, and is automatically invalidated & refreshed each time the sessions list changes.
        /// </summary>
        public IEnumerable<string> TargetAutoCompleteSource => _targetAutoCompleteSource ??= GetSessionAutoCompleteSources();
         //   this.AudioDeviceManagerVM.GetSessionNames(AudioDeviceManagerVM.SessionNameFormat.ProcessIdentifier | AudioDeviceManagerVM.SessionNameFormat.ProcessName);
        public IEnumerable<IHotkeyAction> Actions { get; internal set; } = null!;
        public IEnumerable<string> NotificationModes { get; set; } = Enum.GetNames(typeof(DisplayTarget));
        public IEnumerable<string> AddonDirectories { get; set; } = GetAddonDirectories();
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
        //public AudioDeviceManagerVM AudioDeviceManagerVM { get; }
        public AudioDeviceManagerVM AudioAPI { get; }
        public HotkeyManager HotkeyAPI { get; }
        public ListNotificationVM ListNotificationVM { get; }
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
        /// Gets all of the PIDs, ProcessNames, and ProcessIdentifiers of all managed audio sessions.
        /// </summary>
        /// <returns>Array of strings containing all known PIDs, ProcessNames, and ProcessIdentifiers.</returns>
        private string[] GetSessionAutoCompleteSources()
        {
            List<string> l = new();

            for (int i = 0; i < AudioAPI.Sessions.Count; ++i)
            {
                var sessionVM = AudioAPI.Sessions[i];

                l.Add(sessionVM.PID.ToString());
                l.Add(sessionVM.ProcessName);
                l.Add(sessionVM.ProcessIdentifier);
            }

            return l.ToArray();
        }
        #endregion Methods
    }
}
