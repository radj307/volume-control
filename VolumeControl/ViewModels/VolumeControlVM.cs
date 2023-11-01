using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
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

            // create a template provider manager
            var templateProviderManager = new TemplateProviderManager();
            HotkeyActionAddonLoader.LoadProviders(ref templateProviderManager, GetDefaultDataTemplateProviderTypes());

            var actions = HotkeyActionAddonLoader.LoadActions(templateProviderManager, GetDefaultActionGroupTypes());

            HotkeyAPI.HotkeyManager.HotkeyActionManager.AddActionDefinitions(actions);

            AddonDirectories.ForEach(dir =>
            {
                if (Directory.Exists(dir))
                {
                    FLog.Trace($"[AddonLoader] Searching for DLLs in directory \"{dir}\".");
                    foreach (string dllPath in Directory.EnumerateFiles(dir, "*.dll", new EnumerationOptions() { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = false, }))
                    {
                        // print version info
                        var fileName = Path.GetFileName(dllPath);
                        FLog.Debug($"[AddonLoader] Found addon DLL \"{fileName}\"");

                        var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath);
                        if (versionInfo == null)
                        {
                            FLog.Debug($"[AddonLoader] Addon DLL \"{fileName}\" does not have any version information.");
                        }
                        else
                        {
                            var authorName = versionInfo.CompanyName;

                            if (versionInfo.IsDebug)
                            {
                                FLog.Warning(
                                    $"[AddonLoader] Addon DLL was built in DEBUG configuration \"{dllPath}\"!",
                                    $"              Contact {versionInfo.CompanyName ?? "the addon author"}");
                            }
                            if (FLog.FilterEventType(Log.Enum.EventType.DEBUG))
                            {

                                FLog.Debug($"[AddonLoader] Found addon DLL \"{versionInfo.FileName}\":", versionInfo.ToString());
                            }
                        }

                        // try loading the assembly
                        try
                        {
                            var asm = Assembly.LoadFrom(dllPath);
                            var assemblyName = asm.FullName ?? dllPath;
                            var exportedTypes = asm.GetExportedTypes();
                            asm = null;

                            FLog.Debug($"[AddonLoader] \"{assemblyName}\" exports {exportedTypes.Length} types.");

                            // load providers from addon assembly
                            HotkeyActionAddonLoader.LoadProviders(ref templateProviderManager, exportedTypes);

                            // load actions from addon assembly
                            HotkeyAPI.HotkeyManager.HotkeyActionManager.AddActionDefinitions(HotkeyActionAddonLoader.LoadActions(templateProviderManager, exportedTypes));
                        }
                        catch (Exception ex)
                        {
                            FLog.Critical($"[AddonLoader] Failed to load addon DLL \"{dllPath}\" due to an exception!", ex);
                        }
                    }
                }
                else
                {
                    FLog.Trace($"[AddonLoader] Addon directory \"{dir}\" doesn't exist.");
                }
            });

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
        #endregion Properties

        #region Methods

        #region GetDefaultActionGroupTypes
        /// <summary>
        /// Loads the default hotkey actions from VolumeControl.HotkeyActions
        /// </summary>
        private static Type[] GetDefaultActionGroupTypes()
        {
            var asm = Assembly.Load($"{nameof(VolumeControl)}.{nameof(HotkeyActions)}");

            return asm.GetExportedTypes().Where(type => type.GetCustomAttribute<Core.Attributes.HotkeyActionGroupAttribute>() != null).ToArray();
        }
        /// <summary>
        /// Loads the default data template providers from VolumeControl.SDK
        /// </summary>
        private static Type[] GetDefaultDataTemplateProviderTypes()
        {
            var asm = Assembly.Load($"{nameof(VolumeControl)}.{nameof(SDK)}");

            return asm.GetExportedTypes().Where(type => type.GetCustomAttribute<Core.Attributes.DataTemplateProviderAttribute>() != null).ToArray();
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
